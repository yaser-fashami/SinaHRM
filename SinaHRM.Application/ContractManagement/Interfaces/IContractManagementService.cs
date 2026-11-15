using SinaHRM.Application.ContractManagement.DTOs;
using SinaHRM.Domain.Common;
using SinaHRM.Domain.Entities;
using System.Text.Json;

namespace SinaHRM.Application.ContractManagement.Interfaces;

public interface IContractManagementService
{
    Task<Result<IReadOnlyList<PersonnelListItemDto>>> GetAllPersonnelAsync(CancellationToken cancellationToken);

    Task<Result<PersonnelListItemDto>> GetPersonnelDetailByIdAsync(int id);

    Task<Result> SaveContractTemplate(JsonDocument input);

    Task<Result<EmploymentContractTemplateVersion>> GetEmploymentContractTemplate(string code);

    Task<Result> CreateContract(IReadOnlyList<int> employeeIds, CancellationToken cancellationToken);



    Task<Result<IReadOnlyList<ContractListItemDto>>> GetAllContractAsync(CancellationToken cancellationToken);

    Task<Result<string>> GetContractById(int id, CancellationToken cancellationToken);

    Task<Result<short[]>> GetHRContractKPIs();
}
