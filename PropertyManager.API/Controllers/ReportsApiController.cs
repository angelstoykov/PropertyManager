using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.Reporting;

namespace PropertyManager.API.Controllers;

[Authorize]
[ApiController]
[Route("reports")]
public class ReportsApiController : ControllerBase
{
    private readonly ReportRunner _runner;

    public ReportsApiController(ReportRunner runner)
    {
        _runner = runner;
    }

    [HttpGet]
    public IActionResult GetCatalog()
    {
        return Ok(_runner.ListForUser(CurrentRoles()));
    }

    [HttpPost("{key}/run")]
    public async Task<IActionResult> Run(string key, [FromBody] ReportRunRequest? request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _runner.RunAsync(key, request ?? new ReportRunRequest(), CurrentRoles(), cancellationToken);
            return Ok(result);
        }
        catch (ReportNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ReportValidationException ex)
        {
            return BadRequest(new ReportErrorResponse { Errors = ex.Errors });
        }
    }

    private IEnumerable<string> CurrentRoles() =>
        User.FindAll(ClaimTypes.Role).Select(claim => claim.Value);
}
