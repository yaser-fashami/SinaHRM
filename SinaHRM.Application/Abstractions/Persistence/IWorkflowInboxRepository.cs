using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Application.Abstractions.Persistence;

public interface IWorkflowInboxRepository
{
    Task<WorkflowInboxRecordOutcome> RecordAsync(
        WorkflowInboxMessage message,
        CancellationToken cancellationToken);
}
