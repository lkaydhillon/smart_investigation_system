using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.Features.Analytics;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AnalyticsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("crime-stats")]
    public async Task<IActionResult> GetCrimeStats([FromQuery] Guid? districtId, [FromQuery] Guid? stationId)
    {
        var result = await _mediator.Send(new GetCrimeStatsQuery(districtId, stationId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}
