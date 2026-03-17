using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Application.Features.Cases.Commands;
using SmartInvestigation.Application.Features.Cases.Queries;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CasesController : ControllerBase
{
    private readonly IMediator _mediator;
    public CasesController(IMediator mediator) => _mediator = mediator;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    [HttpGet]
    public async Task<IActionResult> GetCases([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null, [FromQuery] string? search = null,
        [FromQuery] Guid? officerId = null, [FromQuery] Guid? stationId = null)
    {
        var result = await _mediator.Send(new GetCasesQuery(pageNumber, pageSize, status, search, officerId, stationId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCase(Guid id)
    {
        var result = await _mediator.Send(new GetCaseByIdQuery(id));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> CreateCase([FromBody] CreateCaseRequest request)
    {
        var result = await _mediator.Send(new CreateCaseCommand(request, UserId));
        return result.IsSuccess ? StatusCode(201, result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateCaseStatusRequest request)
    {
        var result = await _mediator.Send(new UpdateCaseStatusCommand(id, request.Status, request.Remarks, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("{id:guid}/transfer")]
    public async Task<IActionResult> TransferCase(Guid id, [FromBody] TransferCaseRequest request)
    {
        var result = await _mediator.Send(new TransferCaseCommand(id, request.ToOfficerId, request.ToStationId, request.Reason, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard([FromQuery] Guid? officerId = null, [FromQuery] Guid? stationId = null)
    {
        var result = await _mediator.Send(new GetDashboardStatsQuery(officerId, stationId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}

public record UpdateCaseStatusRequest(Domain.Enums.CaseStatus Status, string? Remarks);
public record TransferCaseRequest(Guid ToOfficerId, Guid? ToStationId, string Reason);
