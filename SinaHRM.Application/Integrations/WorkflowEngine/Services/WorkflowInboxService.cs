using System.Text.Json;
using SinaHRM.Application.Abstractions.Persistence;
using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Application.Integrations.WorkflowEngine.Services;

public sealed class WorkflowInboxService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly IWorkflowInboxRepository _repository;

    public WorkflowInboxService(IWorkflowInboxRepository repository)
    {
        _repository = repository;
    }

    public Task<WorkflowInboxRecordOutcome> RecordAsync(
        WorkflowCompletedEvent message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        Validate(message);

        // Serializing the typed contract gives a stable property order for payload comparison.
        var payloadJson = JsonSerializer.Serialize(message, JsonOptions);
        var inboxMessage = WorkflowInboxMessage.Create(
            message.EventId,
            payloadJson,
            DateTimeOffset.UtcNow);

        return _repository.RecordAsync(inboxMessage, cancellationToken);
    }

    private static void Validate(WorkflowCompletedEvent message)
    {
        if (message.EventId == Guid.Empty)
            throw new ArgumentException("EventId cannot be empty.", nameof(message));

        if (message.WorkflowInstanceId == Guid.Empty)
            throw new ArgumentException("WorkflowInstanceId cannot be empty.", nameof(message));

        ArgumentException.ThrowIfNullOrWhiteSpace(message.BusinessType);
        ArgumentException.ThrowIfNullOrWhiteSpace(message.BusinessReference);

        if (message.CompletedAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("CompletedAtUtc must be UTC.", nameof(message));

        var hasDocumentId = !string.IsNullOrWhiteSpace(message.DocumentId);
        var hasDocumentVersion = !string.IsNullOrWhiteSpace(message.DocumentVersion);
        var hasDocumentHash = !string.IsNullOrWhiteSpace(message.DocumentHash);

        if (hasDocumentId != hasDocumentVersion ||
            hasDocumentId != hasDocumentHash)
        {
            throw new ArgumentException(
                "DocumentId, DocumentVersion and DocumentHash must be provided together.",
                nameof(message));
        }

        if (message.DocumentId?.Length > 200)
            throw new ArgumentException("DocumentId cannot exceed 200 characters.", nameof(message));

        if (message.DocumentVersion?.Length > 100)
            throw new ArgumentException("DocumentVersion cannot exceed 100 characters.", nameof(message));

        if (message.DocumentHash?.Length > 256)
            throw new ArgumentException("DocumentHash cannot exceed 256 characters.", nameof(message));
    }
}
