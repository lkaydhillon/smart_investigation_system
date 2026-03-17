using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.Features.Evidence;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class EvidenceController : ControllerBase
{
    private readonly IMediator _mediator;
    public EvidenceController(IMediator mediator) => _mediator = mediator;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    [HttpGet("case/{caseId:guid}")]
    public async Task<IActionResult> GetByCase(Guid caseId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetEvidenceByCaseQuery(caseId, pageNumber, pageSize));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetEvidenceByIdQuery(id));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Application.DTOs.UploadEvidenceRequest request)
    {
        var result = await _mediator.Send(new CreateEvidenceCommand(request, UserId));
        return result.IsSuccess ? StatusCode(201, result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateEvidenceStatusReq request)
    {
        var result = await _mediator.Send(new UpdateEvidenceStatusCommand(id, request.Status, request.Remarks, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("{id:guid}/chain-of-custody")]
    public async Task<IActionResult> AddChainOfCustody(Guid id, [FromBody] AddCustodyReq request)
    {
        var result = await _mediator.Send(new AddChainOfCustodyCommand(id, request.Action, request.StorageLocation, request.Remarks, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("forensic-request")]
    public async Task<IActionResult> CreateForensicRequest([FromBody] ForensicRequestReq request)
    {
        var result = await _mediator.Send(new CreateForensicLabRequestCommand(request.CaseId, request.EvidenceId, request.LabName, request.RequestedAnalysis, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}

public record UpdateEvidenceStatusReq(EvidenceStatus Status, string? Remarks);
public record AddCustodyReq(string Action, string? StorageLocation, string? Remarks);
public record ForensicRequestReq(Guid CaseId, Guid EvidenceId, string LabName, string? RequestedAnalysis);
