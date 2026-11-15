using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SinaHRM.Application.ContractManagement.DTOs;
using SinaHRM.Application.ContractManagement.Interfaces;
using SinaHRM.Domain.Common;
using System.Text.Json;

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



    #region Personnel
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

    [HttpGet("GetHRContractTemplate")]
    public async Task<ActionResult<IReadOnlyList<PersonnelListItemDto>>> HRContractTemplate(string code)
    {
        var result = await _cmService.GetEmploymentContractTemplate(code);
        return Ok(result.Value.TemplateText);
    }

    [HttpPost("SaveCT")]
    public async Task<ActionResult> SaveContractTemplate([FromBody] JsonDocument body)
    {
        var result = await _cmService.SaveContractTemplate(body);
        if (result.IsSuccess)
        {
            return Ok();
        }
        else
        {
            return Problem(
                title: "Internal Server Error",
                detail: result.Error.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    [HttpPost("StartContractProcess")]
    public async Task<ActionResult> StartContractProcess([FromBody] List<int> ids, CancellationToken cancellationToken)
    {
        var result = await _cmService.CreateContract(ids, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Message);
        }
        else
        {
            return Problem(
                title: "Internal Server Error",
                detail: result.Error.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    #endregion

    #region Contract
    [HttpGet("GetAllContracts")]
    public async Task<ActionResult<IReadOnlyList<ContractListItemDto>>> GetAllContracts(CancellationToken cancellationToken)
    {
        var result = await _cmService.GetAllContractAsync(cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else
        {
            return Problem(
                title: "Internal Server Error",
                detail: result.Error.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    [HttpGet("contract-viewer")]
    public async Task<ActionResult> ContractViewer(int id, CancellationToken cancellationToken)
    {
        var result = await _cmService.GetContractById(id, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else
        {
            return Problem(
                title: "Internal Server Error",
                detail: result.Error.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    [HttpGet("GetHRContractKPIs")]
    public async Task<ActionResult> GetHRContractKPIs()
    {
        var result = await _cmService.GetHRContractKPIs();
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else
        {
            return Problem(
                title: "Internal Server Error",
                detail: result.Error.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
    #endregion
}



