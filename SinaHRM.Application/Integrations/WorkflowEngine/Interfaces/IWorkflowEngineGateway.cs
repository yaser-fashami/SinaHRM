using SinaHRM.Application.ContractManagement.DTOs;
using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;

namespace SinaHRM.Application.Integrations.WorkflowEngine.Interfaces;


public interface IWorkflowEngineGateway
{
    Task<StartWorkflowResult> StartAsync(
        StartContractWorkflowRequest request,
        CancellationToken cancellationToken);
}
