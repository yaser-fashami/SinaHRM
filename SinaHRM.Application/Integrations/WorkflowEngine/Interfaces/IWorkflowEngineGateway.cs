using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;

namespace SinaHRM.Application.Integrations.WorkflowEngine.Interfaces;

public interface IWorkflowEngineGateway
{
    Task<Guid> StartAsync(
        StartContractWorkflowRequest request,
        CancellationToken cancellationToken);
}
