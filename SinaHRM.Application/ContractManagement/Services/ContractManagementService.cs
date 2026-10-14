using SinaHRM.Application.Abstractions.Persistence;
using SinaHRM.Application.ContractManagement.DTOs;
using SinaHRM.Application.ContractManagement.Interfaces;
using SinaHRM.Utilities;

namespace SinaHRM.Application.ContractManagement.Services;

public class ContractManagementService : IContractManagementService
{
    private readonly IContractManagementRepository _CMRepo;

    public ContractManagementService(IContractManagementRepository cmRepo)
    {
        _CMRepo = cmRepo;
    }

    public async Task<Result<IReadOnlyList<PersonnelListItemDto>>> GetAllPersonnelAsync(CancellationToken cancellationToken)
    {
        return await _CMRepo.GetAllPersonnelAsync(cancellationToken);
    }

    public async Task<Result<PersonnelListItemDto>> GetPersonnelDetailByIdAsync(int id)
    {
        return await _CMRepo.GetPersonnelDetailByIdAsync(id);
    }
}
