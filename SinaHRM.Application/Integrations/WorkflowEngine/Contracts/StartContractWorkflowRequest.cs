namespace SinaHRM.Application.Integrations.WorkflowEngine.Contracts;

public sealed record StartContractWorkflowRequest
{
    public required string WorkflowDefinitionCode { get; init; }

    public required string BusinessType { get; init; }

    public required string BusinessReference { get; init; }

    public required string CorrelationId { get; init; }

    public required string IdempotencyKey { get; init; }

    public required int ContractSchemaVersion { get; init; }

    public string? DocumentId { get; init; }

    public string? DocumentVersion { get; init; }

    public string? DocumentHash { get; init; }

    public object? Input { get; init; }
}
