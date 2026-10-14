using Microsoft.AspNetCore.Mvc.RazorPages;
using SinaHRM.WebUI.Models.ContractManagement;
using SinaHRM.WebUI.Services.ContractManagement;

namespace SinaHRM.WebUI.Pages.ContractManagement;

public sealed class PersonnelListModel(
    PersonnelApiClient personnelApiClient,
    ILogger<PersonnelListModel> logger) : PageModel
{
    public IReadOnlyList<PersonnelListItemDto> Personnel { get; private set; } = [];

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Personnel = await personnelApiClient.GetAllAsync(cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(
                exception,
                "Failed to retrieve personnel from the API.");

            ErrorMessage = "Unable to load personnel. Please try again.";
        }
    }
}