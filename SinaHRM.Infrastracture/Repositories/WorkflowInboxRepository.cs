using Microsoft.EntityFrameworkCore;
using SinaHRM.Application.Abstractions.Persistence;
using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;
using SinaHRM.Domain.Entities;
using SinaHRM.Domain.Enums;

namespace SinaHRM.Infrastracture.Data.Sql.Repositories;

public sealed class WorkflowInboxRepository : IWorkflowInboxRepository
{
    private readonly SinaHRMDbContext _dbContext;
    public WorkflowInboxRepository(SinaHRMDbContext dbContext) => _dbContext = dbContext;

    public async Task<WorkflowInboxRecordOutcome> RecordAsync(WorkflowInboxMessage message, CancellationToken cancellationToken)
    {
        var existing = await FindByEventIdAsync(message.EventId, cancellationToken);
        if (existing is not null) return ComparePayload(existing, message);
        await _dbContext.WorkflowInboxMessages.AddAsync(message, cancellationToken);
        try { await _dbContext.SaveChangesAsync(cancellationToken); return WorkflowInboxRecordOutcome.Accepted; }
        catch (DbUpdateException)
        {
            _dbContext.Entry(message).State = EntityState.Detached;
            existing = await FindByEventIdAsync(message.EventId, cancellationToken);
            if (existing is null) throw;
            return ComparePayload(existing, message);
        }
    }

    public async Task<IReadOnlyList<WorkflowInboxMessage>> ClaimBatchAsync(int batchSize, DateTimeOffset nowUtc, DateTimeOffset leaseExpiresAtUtc, int maxAttempts, CancellationToken cancellationToken)
    {
        ValidateClaimArguments(batchSize, nowUtc, leaseExpiresAtUtc, maxAttempts);
        return await _dbContext.WorkflowInboxMessages.FromSqlInterpolated($"""
            ;WITH [Candidates] AS
            (
                SELECT TOP ({batchSize}) *
                FROM [Integration].[WorkflowInboxMessages] WITH (UPDLOCK, READPAST, READCOMMITTEDLOCK)
                WHERE (([Status] = N'Pending' AND [NextAttemptAtUtc] <= {nowUtc})
                    OR ([Status] = N'Processing' AND [LeaseExpiresAtUtc] <= {nowUtc}))
                    AND [AttemptCount] < {maxAttempts}
                ORDER BY CASE WHEN [Status] = N'Pending' THEN [NextAttemptAtUtc] ELSE [LeaseExpiresAtUtc] END, [ReceivedAtUtc], [Id]
            )
            UPDATE [Candidates]
            SET [Status] = N'Processing', [LeaseToken] = NEWID(), [LeaseExpiresAtUtc] = {leaseExpiresAtUtc},
                [AttemptCount] = CASE WHEN [Status] = N'Pending' THEN [AttemptCount] + 1 ELSE [AttemptCount] END,
                [TotalAttemptCount] = CASE WHEN [Status] = N'Pending' THEN [TotalAttemptCount] + 1 ELSE [TotalAttemptCount] END,
                [LastAttemptAtUtc] = CASE WHEN [Status] = N'Pending' THEN {nowUtc} ELSE [LastAttemptAtUtc] END
            OUTPUT inserted.*;
            """).AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<bool> MarkProcessedAsync(Guid eventId, Guid leaseToken, DateTimeOffset processedAtUtc, CancellationToken cancellationToken)
    {
        ValidateLease(eventId, leaseToken, processedAtUtc);
        var affected = await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE [Integration].[WorkflowInboxMessages]
            SET [Status] = N'Processed', [ProcessedAtUtc] = {processedAtUtc}, [LeaseToken] = NULL, [LeaseExpiresAtUtc] = NULL, [LastError] = NULL
            WHERE [EventId] = {eventId} AND [LeaseToken] = {leaseToken} AND [Status] = N'Processing' AND [LeaseExpiresAtUtc] > SYSUTCDATETIME();
            """, cancellationToken);
        return affected == 1;
    }

    public async Task<bool> ScheduleRetryAsync(Guid eventId, Guid leaseToken, DateTimeOffset nextAttemptAtUtc, string error, int maxAttempts, CancellationToken cancellationToken)
    {
        ValidateLease(eventId, leaseToken, nextAttemptAtUtc);
        if (maxAttempts <= 0) throw new ArgumentOutOfRangeException(nameof(maxAttempts));
        var safeError = NormalizeError(error);
        var affected = await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE [Integration].[WorkflowInboxMessages]
            SET [Status] = CASE WHEN [AttemptCount] >= {maxAttempts} THEN N'DeadLetter' ELSE N'Pending' END,
                [NextAttemptAtUtc] = {nextAttemptAtUtc}, [LastError] = {safeError},
                [DeadLetterAtUtc] = CASE WHEN [AttemptCount] >= {maxAttempts} THEN SYSUTCDATETIME() ELSE [DeadLetterAtUtc] END,
                [LeaseToken] = NULL, [LeaseExpiresAtUtc] = NULL
            WHERE [EventId] = {eventId} AND [LeaseToken] = {leaseToken} AND [Status] = N'Processing' AND [LeaseExpiresAtUtc] > SYSUTCDATETIME();
            """, cancellationToken);
        return affected == 1;
    }

    public async Task<bool> MoveToDeadLetterAsync(Guid eventId, Guid leaseToken, string error, CancellationToken cancellationToken)
    {
        ValidateIds(eventId, leaseToken);
        var safeError = NormalizeError(error);
        var affected = await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE [Integration].[WorkflowInboxMessages]
            SET [Status] = N'DeadLetter', [LastError] = {safeError}, [DeadLetterAtUtc] = SYSUTCDATETIME(),
                [LeaseToken] = NULL, [LeaseExpiresAtUtc] = NULL
            WHERE [EventId] = {eventId} AND [LeaseToken] = {leaseToken} AND [Status] = N'Processing' AND [LeaseExpiresAtUtc] > SYSUTCDATETIME();
            """, cancellationToken);
        return affected == 1;
    }

    public async Task<WorkflowInboxCompletionOutcome> ProcessCompletionAsync(
        int contractId,
        Guid workflowInstanceId,
        DateTimeOffset completedAtUtc,
        Guid eventId,
        Guid leaseToken,
        DateTimeOffset processedAtUtc,
        CancellationToken cancellationToken)
    {
        if (contractId <= 0) throw new ArgumentOutOfRangeException(nameof(contractId));
        if (workflowInstanceId == Guid.Empty) throw new ArgumentException("WorkflowInstanceId is required.", nameof(workflowInstanceId));
        ValidateLease(eventId, leaseToken, processedAtUtc);
        EnsureUtc(completedAtUtc, nameof(completedAtUtc));

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var changed = await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE [Contract].[EmploymentContracts]
            SET [Status] = N'Signed', [SignatureCompletedAtUtc] = {completedAtUtc}
            WHERE [Id] = {contractId}
                AND [WorkflowInstanceId] = {workflowInstanceId}
                AND [Status] = N'PendingSignature'
                AND [SignatureCompletedAtUtc] IS NULL
                AND [IsDelete] = 0;
            """, cancellationToken);

        var outcome = WorkflowInboxCompletionOutcome.Processed;
        if (changed == 0)
        {
            var contract = await _dbContext.EmploymentContracts
                .AsNoTracking()
                .Where(x => x.Id == contractId && !x.IsDelete)
                .Select(x => new { x.WorkflowInstanceId, x.Status, x.SignatureCompletedAtUtc })
                .SingleOrDefaultAsync(cancellationToken);

            if (contract?.WorkflowInstanceId == workflowInstanceId &&
                contract.SignatureCompletedAtUtc is not null &&
                (contract.Status == EmploymentContractStatus.Signed || contract.Status == EmploymentContractStatus.Active))
            {
                // A different completion event already won for this contract. Preserve first-wins semantics.
                outcome = WorkflowInboxCompletionOutcome.AlreadyCompleted;
            }
            else
            {
                var deadLetterError = contract is null
                    ? "Employment contract was not found."
                    : contract.WorkflowInstanceId != workflowInstanceId
                        ? "WorkflowInstanceId does not match the employment contract."
                        : "Employment contract is not in PendingSignature state.";

                var deadLettered = await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                    UPDATE [Integration].[WorkflowInboxMessages]
                    SET [Status] = N'DeadLetter', [LastError] = {deadLetterError},
                        [DeadLetterAtUtc] = {processedAtUtc}, [LeaseToken] = NULL, [LeaseExpiresAtUtc] = NULL
                    WHERE [EventId] = {eventId} AND [LeaseToken] = {leaseToken}
                        AND [Status] = N'Processing' AND [LeaseExpiresAtUtc] > {processedAtUtc};
                    """, cancellationToken);

                if (deadLettered != 1)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return WorkflowInboxCompletionOutcome.LeaseLost;
                }

                await transaction.CommitAsync(cancellationToken);
                return WorkflowInboxCompletionOutcome.InvalidContractState;
            }
        }

        var processed = await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE [Integration].[WorkflowInboxMessages]
            SET [Status] = N'Processed', [ProcessedAtUtc] = {processedAtUtc},
                [LeaseToken] = NULL, [LeaseExpiresAtUtc] = NULL, [LastError] = NULL
            WHERE [EventId] = {eventId} AND [LeaseToken] = {leaseToken}
                AND [Status] = N'Processing' AND [LeaseExpiresAtUtc] > {processedAtUtc};
            """, cancellationToken);

        if (processed != 1)
        {
            await transaction.RollbackAsync(cancellationToken);
            return WorkflowInboxCompletionOutcome.LeaseLost;
        }

        await transaction.CommitAsync(cancellationToken);
        return outcome;
    }

    public async Task<bool> ReplayDeadLetterAsync(Guid eventId, DateTimeOffset replayedAtUtc, CancellationToken cancellationToken)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("EventId is required.", nameof(eventId));
        EnsureUtc(replayedAtUtc, nameof(replayedAtUtc));
        var affected = await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE [Integration].[WorkflowInboxMessages]
            SET [Status] = N'Pending', [AttemptCount] = 0, [NextAttemptAtUtc] = {replayedAtUtc},
                [LeaseToken] = NULL, [LeaseExpiresAtUtc] = NULL, [ProcessedAtUtc] = NULL
            WHERE [EventId] = {eventId} AND [Status] = N'DeadLetter';
            """, cancellationToken);
        return affected == 1;
    }

    public async Task<int> ReapExpiredLeasesAsync(int batchSize, int maxAttempts, DateTimeOffset nowUtc, CancellationToken cancellationToken)
    {
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));
        if (maxAttempts <= 0) throw new ArgumentOutOfRangeException(nameof(maxAttempts));
        EnsureUtc(nowUtc, nameof(nowUtc));
        return await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
            ;WITH [Candidates] AS
            (
                SELECT TOP ({batchSize}) *
                FROM [Integration].[WorkflowInboxMessages] WITH (UPDLOCK, READPAST, READCOMMITTEDLOCK)
                WHERE [Status] = N'Processing'
                    AND [LeaseExpiresAtUtc] <= {nowUtc}
                    AND [AttemptCount] >= {maxAttempts}
                ORDER BY [LeaseExpiresAtUtc], [ReceivedAtUtc], [Id]
            )
            UPDATE [Candidates]
            SET [Status] = N'DeadLetter',
                [DeadLetterAtUtc] = {nowUtc},
                [LastError] = N'Lease expired after reaching the maximum attempt count.',
                [LeaseToken] = NULL,
                [LeaseExpiresAtUtc] = NULL;
            """, cancellationToken);
    }

    private Task<WorkflowInboxMessage?> FindByEventIdAsync(Guid eventId, CancellationToken cancellationToken)
        => _dbContext.WorkflowInboxMessages.AsNoTracking().SingleOrDefaultAsync(x => x.EventId == eventId, cancellationToken);

    private static WorkflowInboxRecordOutcome ComparePayload(WorkflowInboxMessage existing, WorkflowInboxMessage incoming)
        => string.Equals(existing.PayloadJson, incoming.PayloadJson, StringComparison.Ordinal) ? WorkflowInboxRecordOutcome.Duplicate : WorkflowInboxRecordOutcome.Conflict;

    private static void ValidateClaimArguments(int batchSize, DateTimeOffset nowUtc, DateTimeOffset leaseExpiresAtUtc, int maxAttempts)
    {
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));
        if (maxAttempts <= 0) throw new ArgumentOutOfRangeException(nameof(maxAttempts));
        EnsureUtc(nowUtc, nameof(nowUtc)); EnsureUtc(leaseExpiresAtUtc, nameof(leaseExpiresAtUtc));
        if (leaseExpiresAtUtc <= nowUtc) throw new ArgumentException("Lease expiration must be later than claim time.", nameof(leaseExpiresAtUtc));
    }

    private static void ValidateLease(Guid eventId, Guid leaseToken, DateTimeOffset timestamp)
    {
        ValidateIds(eventId, leaseToken); EnsureUtc(timestamp, nameof(timestamp));
    }

    private static void ValidateIds(Guid eventId, Guid leaseToken)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("EventId is required.", nameof(eventId));
        if (leaseToken == Guid.Empty) throw new ArgumentException("LeaseToken is required.", nameof(leaseToken));
    }

    private static void EnsureUtc(DateTimeOffset value, string name)
    {
        if (value.Offset != TimeSpan.Zero) throw new ArgumentException("Timestamp must be UTC.", name);
    }

    private static string NormalizeError(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);
        return error.Length <= 4000 ? error : error[..4000];
    }
}