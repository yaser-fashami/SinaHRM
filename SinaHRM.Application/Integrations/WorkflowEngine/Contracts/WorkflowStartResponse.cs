namespace SinaHRM.Application.Integrations.WorkflowEngine.Contracts;

public sealed record WorkflowStartResponse
{
    public required Guid Id { get; init; }
}
