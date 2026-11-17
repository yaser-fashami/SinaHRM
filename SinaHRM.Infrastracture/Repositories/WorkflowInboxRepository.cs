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
}
