namespace SinaHRM.Application.Integrations.WorkflowEngine.Contracts;

public sealed record WorkflowCompletedEvent
{
    public required Guid EventId { get; init; }

    public required Guid WorkflowInstanceId { get; init; }

    public required string BusinessType { get; init; }

    public required string BusinessReference { get; init; }

    public required DateTimeOffset CompletedAtUtc { get; init; }

    public required string DocumentHash { get; init; }
}
