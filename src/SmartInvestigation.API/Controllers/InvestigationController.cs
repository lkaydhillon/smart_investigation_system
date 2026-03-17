using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.Features.Investigation;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InvestigationController : ControllerBase
{
    private readonly IMediator _mediator;
    public InvestigationController(IMediator mediator) => _mediator = mediator;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    // ── Case Diary ──

    [HttpGet("diary/{caseId:guid}")]
    public async Task<IActionResult> GetDiary(Guid caseId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetCaseDiaryQuery(caseId, pageNumber, pageSize));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("diary")]
    public async Task<IActionResult> CreateDiaryEntry([FromBody] CreateDiaryReq request)
    {
        var result = await _mediator.Send(new CreateCaseDiaryEntryCommand(request.CaseId, request.Content, request.AttachmentUrls, UserId));
        return result.IsSuccess ? StatusCode(201, result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── Interrogation ──

    [HttpPost("interrogation")]
    public async Task<IActionResult> CreateInterrogation([FromBody] CreateInterrogationReq request)
    {
        var result = await _mediator.Send(new CreateInterrogationCommand(request.CaseId, request.PersonId,
            request.StartDateTime, request.Location, request.Summary, request.IsLawyerPresent, request.LawyerName, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── Witness Statement ──

    [HttpPost("witness-statement")]
    public async Task<IActionResult> CreateWitnessStatement([FromBody] CreateStatementReq request)
    {
        var result = await _mediator.Send(new CreateWitnessStatementCommand(request.CaseId, request.PersonId,
            request.StatementText, request.StatementType, request.Location, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── Investigation Steps ──

    [HttpGet("steps/{caseId:guid}")]
    public async Task<IActionResult> GetSteps(Guid caseId)
    {
        var result = await _mediator.Send(new GetInvestigationStepsQuery(caseId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPut("steps/{stepId:guid}")]
    public async Task<IActionResult> UpdateStep(Guid stepId, [FromBody] UpdateStepReq request)
    {
        var result = await _mediator.Send(new UpdateInvestigationStepCommand(stepId, request.Status, request.CompletionRemarks, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── Scene Survey ──

    [HttpPost("scene-survey")]
    public async Task<IActionResult> CreateSceneSurvey([FromBody] CreateSurveyReq request)
    {
        var result = await _mediator.Send(new CreateSceneSurveyCommand(request.CaseId, request.Description,
            request.GpsLatitude, request.GpsLongitude, request.WeatherConditions, request.Observations, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── Investigation Team ──

    [HttpPost("team")]
    public async Task<IActionResult> AssignTeam([FromBody] AssignTeamReq request)
    {
        var result = await _mediator.Send(new AssignInvestigationTeamCommand(request.CaseId, request.LeadOfficerId,
            request.TeamName, request.MemberUserIds, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}

public record CreateDiaryReq(Guid CaseId, string Content, string? AttachmentUrls);
public record CreateInterrogationReq(Guid CaseId, Guid PersonId, DateTime StartDateTime, string? Location, string? Summary, bool IsLawyerPresent, string? LawyerName);
public record CreateStatementReq(Guid CaseId, Guid PersonId, string StatementText, string? StatementType, string? Location);
public record UpdateStepReq(InvestigationStepStatus Status, string? CompletionRemarks);
public record CreateSurveyReq(Guid CaseId, string Description, double? GpsLatitude, double? GpsLongitude, string? WeatherConditions, string? Observations);
public record AssignTeamReq(Guid CaseId, Guid LeadOfficerId, string? TeamName, List<Guid> MemberUserIds);
