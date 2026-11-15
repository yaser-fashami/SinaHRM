using SinaHRM.Application.ContractManagement.DTOs;
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

    Task<Result<EmploymentContract>> SaveContractWithGeneratedContractNumberAsync(EmploymentContract contract,
        CancellationToken cancellationToken = default);
}