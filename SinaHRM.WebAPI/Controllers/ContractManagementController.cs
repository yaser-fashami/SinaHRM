using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SinaHRM.Application.ContractManagement.DTOs;
using SinaHRM.Application.ContractManagement.Interfaces;
using SinaHRM.Domain.Entities;

namespace SinaHRM.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ContractManagement")]
public class ContractManagementController : ControllerBase
{
    private readonly IContractManagementService _cmService;

    public ContractManagementController(IContractManagementService cmService)
    {
        _cmService = cmService;
    }



    [HttpGet("GetAllPersonnels")]
    public async Task<ActionResult<IReadOnlyList<PersonnelListItemDto>>> GetAllPersonnels(CancellationToken cancellationToken)
    {
        var personnels = await _cmService.GetAllPersonnelAsync(cancellationToken);
        return Ok(personnels.Value);
    }

    [HttpGet("GetPersonnel")]
    public async Task<ActionResult<IReadOnlyList<PersonnelListItemDto>>> GetPersonnel(int id)
    {
        var personnel = await _cmService.GetPersonnelDetailByIdAsync(id);
        return Ok(personnel.Value);
    }

}



