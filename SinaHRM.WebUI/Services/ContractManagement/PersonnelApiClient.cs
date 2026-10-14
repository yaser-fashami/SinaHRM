using SinaHRM.WebUI.Models.ContractManagement;

namespace SinaHRM.WebUI.Services.ContractManagement;

public sealed class PersonnelApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<PersonnelListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
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

        return personnel;
    }
}