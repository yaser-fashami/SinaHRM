using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;
using SinaHRM.Application.Integrations.WorkflowEngine.Services;

namespace SinaHRM.API.Controllers;

/// <summary>
/// Receives WorkflowEngine completion callbacks into the durable Inbox.
/// This endpoint is intentionally Development-only until a production identity provider is configured.
/// </summary>
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/integrations/workflow")]
[Produces("application/json")]
public sealed class WorkflowCallbacksController : ControllerBase
{
    private readonly WorkflowInboxService _inboxService;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public WorkflowCallbacksController(
        WorkflowInboxService inboxService,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        _inboxService = inboxService;
        _configuration = configuration;
        _environment = environment;
    }

    [HttpPost("completed")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ReceiveCompletedAsync(
        [FromBody] WorkflowCompletedEvent message,
        CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Workflow callback authentication is not configured for this environment.");
        }

        if (!HasValidDevelopmentBearerToken())
            return Unauthorized();

        try
        {
            var outcome = await _inboxService.RecordAsync(message, cancellationToken);
            return outcome switch
            {
                WorkflowInboxRecordOutcome.Accepted => Accepted(),
                WorkflowInboxRecordOutcome.Duplicate => Accepted(),
                WorkflowInboxRecordOutcome.Conflict => Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "EventId payload conflict",
                    Detail = "The EventId has already been recorded with a different payload."
                }),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid workflow completion callback",
                Detail = exception.Message
            });
        }
    }

    private bool HasValidDevelopmentBearerToken()
    {
        var expectedToken = _configuration["Authentication:DevelopmentBearerToken"];
        if (string.IsNullOrWhiteSpace(expectedToken))
            return false;

        var authorization = Request.Headers.Authorization.ToString();
        const string prefix = "Bearer ";
        if (!authorization.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return false;

        var suppliedBytes = Encoding.UTF8.GetBytes(authorization[prefix.Length..].Trim());
        var expectedBytes = Encoding.UTF8.GetBytes(expectedToken);

        return suppliedBytes.Length > 0 &&
               CryptographicOperations.FixedTimeEquals(expectedBytes, suppliedBytes);
    }
}
