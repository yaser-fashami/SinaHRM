namespace SinaHRM.Application.Integrations.WorkflowEngine.Contracts;

public sealed record StartContractWorkflowRequest
{
    public required string BusinessReference { get; init; }

    public required string WorkflowDefinitionCode { get; init; }

    public required string DocumentTitle { get; init; }

    public required string DocumentContent { get; init; }

    public required string DocumentHash { get; init; }

    public required string InitiatorId { get; init; }

    public required string CorrelationId { get; init; }
}
