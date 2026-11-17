using SinaHRM.Domain.Enums;

namespace SinaHRM.Domain.Entities;

public sealed class WorkflowInboxMessage
{
    private WorkflowInboxMessage()
    {
    }

    public int Id { get; private set; }

    public Guid EventId { get; private set; }

    public string PayloadJson { get; private set; } = null!;

    public WorkflowInboxStatus Status { get; private set; }

    public DateTimeOffset ReceivedAtUtc { get; private set; }

    public static WorkflowInboxMessage Create(
        Guid eventId,
        string payloadJson,
        DateTimeOffset receivedAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(payloadJson);

        if (eventId == Guid.Empty)
        {
            throw new ArgumentException("EventId cannot be empty.", nameof(eventId));
        }

        return new WorkflowInboxMessage
        {
            EventId = eventId,
            PayloadJson = payloadJson,
            Status = WorkflowInboxStatus.Pending,
            ReceivedAtUtc = receivedAtUtc
        };
    }
}
