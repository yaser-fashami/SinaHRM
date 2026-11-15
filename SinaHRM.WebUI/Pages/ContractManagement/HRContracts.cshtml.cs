using Microsoft.AspNetCore.Mvc.RazorPages;
using SinaHRM.WebUI.Models.ContractManagement;
using SinaHRM.WebUI.Services.Interfaces;

namespace SinaHRM.WebUI.Pages.ContractManagement
{
    public class HRContractsModel(IContractManagementApiClient contractApiClient, ILogger<PersonnelListModel> logger) : PageModel
    {
        public IReadOnlyList<ContractListItemDto> Contracts { get; private set; } = [];
        public short DraftKpi { get; private set; }
        public short ActiveKpi { get; private set; }
        public short WaitingToSignKpi { get; private set; }

        public string? ErrorMessage { get; private set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
			try
			{
                Contracts = await contractApiClient.GetAllContractAsync(cancellationToken);
                var kpis = await contractApiClient.GetHRContractKPIsAsync();
                DraftKpi = kpis[0];
                ActiveKpi = kpis[1];
                WaitingToSignKpi = kpis[2];
			}
			catch (HttpRequestException exception)
			{
                logger.LogError(
                    exception,
                    "Failed to retrieve data from the API.");

                ErrorMessage = "Unable to load data. Please try again.";
            }
        }
    }
}
