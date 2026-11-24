using SinaHRM.Domain.Common;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Application.ContractManagement.Interfaces;

public interface IEmploymentContractRepository
{
    Task<string> GetContractNumberAsync(
        bool isTehran,
        DateOnly contractDate,
        CancellationToken cancellationToken = default);

    Task<DateOnly?> GetLatestValidContractEndDateAsync(
        int employeeId,
        CancellationToken cancellationToken = default);

    Task<Result<EmploymentContract>> SaveContractWithGeneratedContractNumberAsync(
        EmploymentContract contract,
        CancellationToken cancellationToken = default);

    Task<EmploymentContract?> GetForWorkflowStartAsync(
        int contractId,
        CancellationToken cancellationToken);

    Task<bool> TrySetWorkflowInstanceIdAsync(
        int contractId,
        Guid workflowInstanceId,
        CancellationToken cancellationToken);
}
