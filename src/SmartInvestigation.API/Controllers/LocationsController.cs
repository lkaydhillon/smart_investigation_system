using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.Features.Locations;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly IMediator _mediator;
    public LocationsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("states")]
    public async Task<IActionResult> GetStates()
    {
        var result = await _mediator.Send(new GetStatesQuery());
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpGet("states/{stateId:guid}/districts")]
    public async Task<IActionResult> GetDistricts(Guid stateId)
    {
        var result = await _mediator.Send(new GetDistrictsQuery(stateId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpGet("stations")]
    public async Task<IActionResult> GetPoliceStations([FromQuery] Guid? districtId = null)
    {
        var result = await _mediator.Send(new GetPoliceStationsQuery(districtId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}
