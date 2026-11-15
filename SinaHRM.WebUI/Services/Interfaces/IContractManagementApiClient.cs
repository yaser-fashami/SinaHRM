using SinaHRM.WebUI.Models.ContractManagement;

namespace SinaHRM.WebUI.Services.Interfaces;

public interface IContractManagementApiClient
{
    public Task<IReadOnlyList<PersonnelListItemDto>> GetAllPersonnelAsync(CancellationToken cancellationToken);

    public Task<PersonnelListItemDto> GetPersonnelAsync(int id, CancellationToken cancellationToken);

    public Task<bool> StartContractProcess(IReadOnlyList<int> ids, CancellationToken cancellationToken);

    public Task<IReadOnlyList<ContractListItemDto>> GetAllContractAsync(CancellationToken cancellationToken);

    public Task<short[]> GetHRContractKPIsAsync();

}
