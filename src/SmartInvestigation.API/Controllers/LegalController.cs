using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.Features.Legal;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class LegalController : ControllerBase
{
    private readonly IMediator _mediator;
    public LegalController(IMediator mediator) => _mediator = mediator;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    // ── Legal Sections ──

    [HttpGet("sections")]
    public async Task<IActionResult> SearchSections([FromQuery] string? searchTerm, [FromQuery] string? act,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new SearchLegalSectionsQuery(searchTerm, act, pageNumber, pageSize));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("case-sections")]
    public async Task<IActionResult> AddSectionToCase([FromBody] AddSectionReq request)
    {
        var result = await _mediator.Send(new AddLegalSectionToCaseCommand(request.CaseId, request.LegalSectionId,
            request.IsAIRecommended, request.ConfidenceScore, request.Remarks, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── Court Hearings ──

    [HttpPost("hearings")]
    public async Task<IActionResult> CreateHearing([FromBody] CreateHearingReq request)
    {
        var result = await _mediator.Send(new CreateCourtHearingCommand(request.CaseId, request.CourtId, request.HearingDate, request.Purpose, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPut("hearings/{hearingId:guid}/outcome")]
    public async Task<IActionResult> RecordOutcome(Guid hearingId, [FromBody] RecordOutcomeReq request)
    {
        var result = await _mediator.Send(new RecordCourtHearingOutcomeCommand(hearingId, request.Outcome,
            request.NextHearingDate, request.JudgeName, request.Remarks, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── Chargesheet ──

    [HttpPost("chargesheets")]
    public async Task<IActionResult> CreateChargesheet([FromBody] CreateChargesheetReq request)
    {
        var result = await _mediator.Send(new CreateChargesheetCommand(request.CaseId, request.AccusedPersonIds,
            request.FileUrl, request.IsSupplementary, request.OriginalChargesheetId, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── Bail ──

    [HttpPost("bail")]
    public async Task<IActionResult> FileBailApplication([FromBody] FileBailReq request)
    {
        var result = await _mediator.Send(new FileBailApplicationCommand(request.CaseId, request.PersonId,
            request.CourtId, request.BailAmount, request.Conditions, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── Remand ──

    [HttpPost("remand")]
    public async Task<IActionResult> CreateRemand([FromBody] CreateRemandReq request)
    {
        var result = await _mediator.Send(new CreateRemandCommand(request.CaseId, request.PersonId,
            request.RemandType, request.StartDate, request.EndDate, request.CourtId, request.CustodyLocation, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}

public record AddSectionReq(Guid CaseId, Guid LegalSectionId, bool IsAIRecommended, double? ConfidenceScore, string? Remarks);
public record CreateHearingReq(Guid CaseId, Guid CourtId, DateTime HearingDate, string? Purpose);
public record RecordOutcomeReq(string? Outcome, DateTime? NextHearingDate, string? JudgeName, string? Remarks);
public record CreateChargesheetReq(Guid CaseId, List<Guid> AccusedPersonIds, string? FileUrl, bool IsSupplementary, Guid? OriginalChargesheetId);
public record FileBailReq(Guid CaseId, Guid PersonId, Guid CourtId, decimal? BailAmount, string? Conditions);
public record CreateRemandReq(Guid CaseId, Guid PersonId, RemandType RemandType, DateTime StartDate, DateTime EndDate, Guid CourtId, string? CustodyLocation);
