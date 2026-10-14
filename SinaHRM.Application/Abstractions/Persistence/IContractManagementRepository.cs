using SinaHRM.Application.ContractManagement.DTOs;
using SinaHRM.Utilities;

namespace SinaHRM.Application.Abstractions.Persistence;

public interface IContractManagementRepository
{
    Task<Result<IReadOnlyList<PersonnelListItemDto>>> GetAllPersonnelAsync(CancellationToken cancellationToken);

    Task<Result<PersonnelListItemDto>> GetPersonnelDetailByIdAsync(int id);
}
