using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Application.Features.Documents;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public DocumentsController(IMediator mediator) => _mediator = mediator;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplates([FromQuery] Guid? categoryId = null)
    {
        var result = await _mediator.Send(new GetTemplatesQuery(categoryId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("templates")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateReq request)
    {
        var result = await _mediator.Send(new CreateTemplateCommand(request.Name, request.CategoryId,
            request.Description, request.Format, request.TemplateContent, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpGet("case/{caseId:guid}")]
    public async Task<IActionResult> GetCaseDocuments(Guid caseId)
    {
        var result = await _mediator.Send(new GetGeneratedDocumentsQuery(caseId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateDocument([FromBody] GenerateDocumentRequest request)
    {
        var result = await _mediator.Send(new GenerateDocumentCommand(request, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}

public record CreateTemplateReq(string Name, Guid? CategoryId, string? Description, string Format, string TemplateContent);
