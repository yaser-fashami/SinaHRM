using System.Globalization;
using System.Text.Json;
using SinaHRM.Application.Abstractions.Persistence;
using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Application.Integrations.WorkflowEngine.Services;

public sealed class WorkflowInboxProcessor
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const string EmploymentContractBusinessType = "EmploymentContract";
    private readonly IWorkflowInboxRepository _repository;

    public WorkflowInboxProcessor(IWorkflowInboxRepository repository)
    {
        _repository = repository;
    }

    public async Task<WorkflowInboxCompletionOutcome> ProcessAsync(
        WorkflowInboxMessage inboxMessage,
        DateTimeOffset processedAtUtc,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(inboxMessage);
        if (inboxMessage.LeaseToken is not Guid leaseToken || leaseToken == Guid.Empty)
            return WorkflowInboxCompletionOutcome.LeaseLost;

        WorkflowCompletedEvent? completion;
        try
        {
            completion = JsonSerializer.Deserialize<WorkflowCompletedEvent>(inboxMessage.PayloadJson, JsonOptions);
        }
        catch (JsonException)
        {
            await _repository.MoveToDeadLetterAsync(
                inboxMessage.EventId, leaseToken, "Inbox payload is not a valid WorkflowCompletedEvent JSON document.", cancellationToken);
            return WorkflowInboxCompletionOutcome.InvalidContractState;
        }

        if (completion is null ||
            completion.EventId != inboxMessage.EventId ||
            completion.WorkflowInstanceId == Guid.Empty ||
            !string.Equals(completion.BusinessType, EmploymentContractBusinessType, StringComparison.Ordinal) ||
            !int.TryParse(completion.BusinessReference, NumberStyles.None, CultureInfo.InvariantCulture, out var contractId) ||
            contractId <= 0 ||
            !string.Equals(completion.BusinessReference, contractId.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal) ||
            completion.CompletedAtUtc.Offset != TimeSpan.Zero)
        {
            await _repository.MoveToDeadLetterAsync(
                inboxMessage.EventId, leaseToken, "Workflow completion payload contains an unsupported business reference or invalid required fields.", cancellationToken);
            return WorkflowInboxCompletionOutcome.InvalidContractState;
        }

        var hasDocumentId = !string.IsNullOrWhiteSpace(completion.DocumentId);
        var hasDocumentVersion = !string.IsNullOrWhiteSpace(completion.DocumentVersion);
        var hasDocumentHash = !string.IsNullOrWhiteSpace(completion.DocumentHash);
        if (hasDocumentId != hasDocumentVersion || hasDocumentId != hasDocumentHash)
        {
            await _repository.MoveToDeadLetterAsync(
                inboxMessage.EventId, leaseToken, "DocumentId, DocumentVersion and DocumentHash must be provided together.", cancellationToken);
            return WorkflowInboxCompletionOutcome.InvalidContractState;
        }

        return await _repository.ProcessCompletionAsync(
            contractId,
            completion.WorkflowInstanceId,
            completion.CompletedAtUtc,
            inboxMessage.EventId,
            leaseToken,
            processedAtUtc,
            cancellationToken);
    }
}
