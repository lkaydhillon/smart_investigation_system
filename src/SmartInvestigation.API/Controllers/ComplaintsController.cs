using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Application.Features.Complaints;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ComplaintsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ComplaintsController(IMediator mediator) => _mediator = mediator;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    [HttpGet]
    public async Task<IActionResult> GetComplaints([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null, [FromQuery] Guid? stationId = null)
    {
        var result = await _mediator.Send(new GetComplaintsQuery(pageNumber, pageSize, status, stationId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> CreateComplaint([FromBody] CreateComplaintRequest request)
    {
        var result = await _mediator.Send(new CreateComplaintCommand(request, UserId));
        return result.IsSuccess ? StatusCode(201, result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateComplaint(Guid id, [FromBody] CreateComplaintRequest request)
    {
        var result = await _mediator.Send(new UpdateComplaintCommand(id, request, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteComplaint(Guid id)
    {
        var result = await _mediator.Send(new DeleteComplaintCommand(id, UserId));
        return result.IsSuccess ? Ok(new { success = true }) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}
