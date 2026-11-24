using System.Globalization;
using SinaHRM.Application.ContractManagement.Interfaces;
using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;
using SinaHRM.Application.Integrations.WorkflowEngine.Interfaces;
using SinaHRM.Domain.Enums;

namespace SinaHRM.Application.Integrations.WorkflowEngine.Services;

public sealed class ContractWorkflowService : IContractWorkflowService
{
    private const string WorkflowDefinitionCode = "CONTRACT_APPROVAL";
    private const string BusinessType = "EmploymentContract";
    private const int ContractSchemaVersion = 1;

    private readonly IEmploymentContractRepository _contractRepository;
    private readonly IWorkflowEngineGateway _workflowEngineGateway;

    public ContractWorkflowService(
        IEmploymentContractRepository contractRepository,
        IWorkflowEngineGateway workflowEngineGateway)
    {
        _contractRepository = contractRepository;
        _workflowEngineGateway = workflowEngineGateway;
    }

    public async Task<Guid> StartForDraftContractAsync(
        int contractId,
        CancellationToken cancellationToken)
    {
        if (contractId <= 0)
            throw new ArgumentOutOfRangeException(nameof(contractId));

        var contract = await _contractRepository.GetForWorkflowStartAsync(
            contractId,
            cancellationToken);

        if (contract is null)
            throw new KeyNotFoundException($"Employment contract '{contractId}' was not found.");

        if (contract.WorkflowInstanceId is Guid existingWorkflowInstanceId)
            return existingWorkflowInstanceId;

        if (contract.Status != EmploymentContractStatus.Draft)
            throw new InvalidOperationException(
                "Only a draft employment contract without an existing workflow can be submitted.");

        var businessReference = contract.Id.ToString(CultureInfo.InvariantCulture);
        var idempotencyKey = $"employment-contract:{businessReference}:workflow-start:v1";

        var request = new StartContractWorkflowRequest
        {
            WorkflowDefinitionCode = WorkflowDefinitionCode,
            BusinessType = BusinessType,
            BusinessReference = businessReference,
            CorrelationId = idempotencyKey,
            IdempotencyKey = idempotencyKey,
            ContractSchemaVersion = ContractSchemaVersion
        };

        // Stable idempotency protects retries if WorkflowEngine accepted the request
        // but SinaHRM failed before persisting the returned instance id.
        var workflowInstanceId = await _workflowEngineGateway.StartAsync(
            request,
            cancellationToken);

        if (workflowInstanceId == Guid.Empty)
            throw new InvalidOperationException("WorkflowEngine returned an empty workflow instance id.");

        var persisted = await _contractRepository.TrySetWorkflowInstanceIdAsync(
            contractId,
            workflowInstanceId,
            cancellationToken);

        if (persisted)
            return workflowInstanceId;

        // Another caller may have completed the same idempotent request concurrently.
        var latest = await _contractRepository.GetForWorkflowStartAsync(
            contractId,
            cancellationToken);

        if (latest?.WorkflowInstanceId == workflowInstanceId)
            return workflowInstanceId;

        throw new InvalidOperationException(
            "WorkflowEngine started the workflow, but SinaHRM could not associate that instance with the contract. Retry with the same idempotency key or reconcile the contract.");
    }
}
