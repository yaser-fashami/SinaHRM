using SinaHRM.Application.ContractManagement.DTOs;
using SinaHRM.Domain.Common;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Application.Abstractions.Persistence;

public interface IContractManagementRepository
{
    Task<Result<IReadOnlyList<PersonnelListItemDto>>> GetAllPersonnelAsync(CancellationToken cancellationToken);

    Task<Result<PersonnelListItemDto>> GetPersonnelDetailByIdAsync(int id);

    Task<Result<IReadOnlyList<Employee>>> GetPersonnelsDetailByIdsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken);



    Task<Result> SaveContractTemplateAsync(string text, string code, short newVersion);

    Task<Result<EmploymentContractTemplateVersion>> GetContractTemplateByCodeAsync(string code);

    Task<Result<IReadOnlyList<ContractListItemDto>>> GetAllContractAsync(CancellationToken cancellationToken);

    Task<Result<string>> GetContractByIdAsync(int id, CancellationToken cancellationToken);

    Task<short[]> GetHRContractKPIsAsync();
}
