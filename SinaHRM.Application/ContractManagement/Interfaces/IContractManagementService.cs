using SinaHRM.Application.ContractManagement.DTOs;
using SinaHRM.Utilities;

namespace SinaHRM.Application.ContractManagement.Interfaces;

public interface IContractManagementService
{
    Task<Result<IReadOnlyList<PersonnelListItemDto>>> GetAllPersonnelAsync(CancellationToken cancellationToken);

    Task<Result<PersonnelListItemDto>> GetPersonnelDetailByIdAsync(int id);
}
