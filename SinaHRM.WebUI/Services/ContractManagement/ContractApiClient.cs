
using SinaHRM.WebUI.Models.ContractManagement;
using SinaHRM.WebUI.Services.Interfaces;

namespace SinaHRM.WebUI.Services.ContractManagement;

public sealed class ContractApiClient(HttpClient httpClient) : IContractManagementApiClient
{

    public async Task<IReadOnlyList<PersonnelListItemDto>> GetAllPersonnelAsync(CancellationToken cancellationToken = default)
    {
        var personnel =
            await httpClient.GetFromJsonAsync<List<PersonnelListItemDto>>(
                "api/v1.0/ContractManagement/GetAllPersonnels",
                cancellationToken);

        return personnel ?? [];
    }

    public async Task<PersonnelListItemDto> GetPersonnelAsync(int id, CancellationToken cancellationToken = default)
    {
        var personnel =
            await httpClient.GetFromJsonAsync<PersonnelListItemDto>(
                "api/v1.0/ContractManagement/GetPersonnel",
                cancellationToken);

        return personnel ?? null!;
    }

    public async Task<bool> StartContractProcess(IReadOnlyList<int> ids, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<bool>(
                "api/v1.0/ContractManagement/StartContractProcess",
                cancellationToken);

        return result;
    }


    public async Task<IReadOnlyList<ContractListItemDto>> GetAllContractAsync(CancellationToken cancellationToken)
    {
        var contracts = await httpClient.GetFromJsonAsync<List<ContractListItemDto>>(
                                    "api/v1.0/ContractManagement/GetAllContracts",
                                    cancellationToken);

        return contracts ?? [];
    }


    public async Task<short[]> GetHRContractKPIsAsync()
    {
        return await httpClient.GetFromJsonAsync<short[]>(
                            "api/v1.0/ContractManagement/GetHRContractKPIs") ?? [];
    }
}
