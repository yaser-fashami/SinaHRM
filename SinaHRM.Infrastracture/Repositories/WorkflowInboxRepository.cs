using Microsoft.EntityFrameworkCore;
using SinaHRM.Application.Abstractions.Persistence;
using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Infrastracture.Data.Sql.Repositories;

public sealed class WorkflowInboxRepository : IWorkflowInboxRepository
{
    private readonly SinaHRMDbContext _dbContext;

    public WorkflowInboxRepository(SinaHRMDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WorkflowInboxRecordOutcome> RecordAsync(
        WorkflowInboxMessage message,
        CancellationToken cancellationToken)
    {
        var existing = await FindByEventIdAsync(message.EventId, cancellationToken);
        if (existing is not null)
            return ComparePayload(existing, message);

        await _dbContext.WorkflowInboxMessages.AddAsync(message, cancellationToken);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return WorkflowInboxRecordOutcome.Accepted;
        }
        catch (DbUpdateException)
        {
            // A concurrent request may have inserted the same EventId after the initial read.
            _dbContext.Entry(message).State = EntityState.Detached;

            existing = await FindByEventIdAsync(message.EventId, cancellationToken);
            if (existing is null)
                throw;

            return ComparePayload(existing, message);
        }
    }

    public async Task<IReadOnlyList<WorkflowInboxMessage>> ClaimBatchAsync(
        int batchSize,
        DateTimeOffset nowUtc,
        DateTimeOffset leaseExpiresAtUtc,
        int maxAttempts,
        CancellationToken cancellationToken)
    {
        ValidateClaimArguments(batchSize, nowUtc, leaseExpiresAtUtc, maxAttempts);

        // Pending messages begin a new processing attempt. Reclaiming an expired lease
        // resumes the same attempt and does not increment AttemptCount.
        return await _dbContext.WorkflowInboxMessages
            .FromSqlInterpolated($"""
                ;WITH [Candidates] AS
                (
                    SELECT TOP ({batchSize}) *
                    FROM [Integration].[WorkflowInboxMessages]
                        WITH (UPDLOCK, READPAST, READCOMMITTEDLOCK)
                    WHERE
                        (
                            (
                                [Status] = N'Pending'
                                AND [NextAttemptAtUtc] <= {nowUtc}
                            )
                            OR
                            (
                                [Status] = N'Processing'
                                AND [LeaseExpiresAtUtc] <= {nowUtc}
                            )
                        )
                        AND [AttemptCount] < {maxAttempts}
                    ORDER BY
                        CASE
                            WHEN [Status] = N'Pending'
                                THEN [NextAttemptAtUtc]
                            ELSE [LeaseExpiresAtUtc]
                        END,
                        [ReceivedAtUtc],
                        [Id]
                )
                UPDATE [Candidates]
                SET
                    [Status] = N'Processing',
                    [LeaseToken] = NEWID(),
                    [LeaseExpiresAtUtc] = {leaseExpiresAtUtc},
                    [AttemptCount] = CASE
                        WHEN [Status] = N'Pending'
                            THEN [AttemptCount] + 1
                        ELSE [AttemptCount]
                    END,
                    [LastAttemptAtUtc] = CASE
                        WHEN [Status] = N'Pending'
                            THEN {nowUtc}
                        ELSE [LastAttemptAtUtc]
                    END
                OUTPUT inserted.*;
                """)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    private Task<WorkflowInboxMessage?> FindByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        return _dbContext.WorkflowInboxMessages
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.EventId == eventId, cancellationToken);
    }

    private static WorkflowInboxRecordOutcome ComparePayload(
        WorkflowInboxMessage existing,
        WorkflowInboxMessage incoming)
    {
        return string.Equals(
            existing.PayloadJson,
            incoming.PayloadJson,
            StringComparison.Ordinal)
            ? WorkflowInboxRecordOutcome.Duplicate
            : WorkflowInboxRecordOutcome.Conflict;
    }

    private static void ValidateClaimArguments(
        int batchSize,
        DateTimeOffset nowUtc,
        DateTimeOffset leaseExpiresAtUtc,
        int maxAttempts)
    {
        if (batchSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(batchSize));

        if (maxAttempts <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxAttempts));

        if (leaseExpiresAtUtc <= nowUtc)
            throw new ArgumentException(
                "Lease expiration must be later than the claim time.",
                nameof(leaseExpiresAtUtc));

        if (nowUtc.Offset != TimeSpan.Zero || leaseExpiresAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Claim timestamps must be UTC.");
    }
}
