using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;
using SinaHRM.Domain.Entities;
namespace SinaHRM.Application.Abstractions.Persistence;
public interface IWorkflowInboxRepository
{
    Task<WorkflowInboxRecordOutcome> RecordAsync(WorkflowInboxMessage message, CancellationToken cancellationToken);
    Task<IReadOnlyList<WorkflowInboxMessage>> ClaimBatchAsync(int batchSize, DateTimeOffset nowUtc, DateTimeOffset leaseExpiresAtUtc, int maxAttempts, CancellationToken cancellationToken);
    Task<bool> MarkProcessedAsync(Guid eventId, Guid leaseToken, DateTimeOffset processedAtUtc, CancellationToken cancellationToken);
    Task<bool> ScheduleRetryAsync(Guid eventId, Guid leaseToken, DateTimeOffset nextAttemptAtUtc, string error, int maxAttempts, CancellationToken cancellationToken);
    Task<bool> MoveToDeadLetterAsync(Guid eventId, Guid leaseToken, string error, CancellationToken cancellationToken);
}