using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.Features.DynamicEntities;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/dynamic-entities")]
[Authorize]
public class DynamicEntitiesController : ControllerBase
{
    private readonly IMediator _mediator;
    public DynamicEntitiesController(IMediator mediator) => _mediator = mediator;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    // ── Entity Definitions ──

    [HttpGet]
    public async Task<IActionResult> GetDefinitions()
    {
        var result = await _mediator.Send(new GetDynamicEntityDefinitionsQuery());
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> CreateDefinition([FromBody] CreateDynEntityReq request)
    {
        var result = await _mediator.Send(new CreateDynamicEntityDefinitionCommand(
            request.Name, request.DisplayName, request.Description, request.IconName, request.Fields, UserId));
        return result.IsSuccess ? StatusCode(201, result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("{entityDefId:guid}/fields")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> AddField(Guid entityDefId, [FromBody] AddDynFieldReq request)
    {
        var result = await _mediator.Send(new AddFieldToEntityCommand(entityDefId, request.FieldName,
            request.DisplayLabel, request.FieldType, request.IsRequired, request.Options,
            request.DefaultValue, request.DisplayOrder, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── Records ──

    [HttpGet("{entityDefId:guid}/records")]
    public async Task<IActionResult> GetRecords(Guid entityDefId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetDynamicRecordsQuery(entityDefId, pageNumber, pageSize));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("{entityDefId:guid}/records")]
    public async Task<IActionResult> SaveRecord(Guid entityDefId, [FromBody] SaveDynRecordReq request)
    {
        var result = await _mediator.Send(new SaveDynamicRecordCommand(entityDefId, request.DisplayTitle, request.FieldValues, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPut("records/{recordId:guid}")]
    public async Task<IActionResult> UpdateRecord(Guid recordId, [FromBody] UpdateDynRecordReq request)
    {
        var result = await _mediator.Send(new UpdateDynamicRecordCommand(recordId, request.DisplayTitle, request.FieldValues, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpDelete("records/{recordId:guid}")]
    public async Task<IActionResult> DeleteRecord(Guid recordId)
    {
        var result = await _mediator.Send(new DeleteDynamicRecordCommand(recordId, UserId));
        return result.IsSuccess ? Ok(new { message = "Deleted" }) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}

public record CreateDynEntityReq(string Name, string DisplayName, string? Description,
    string? IconName, List<CreateDynFieldReq> Fields);

public record AddDynFieldReq(string FieldName, string DisplayLabel, DynamicEntityFieldType FieldType,
    bool IsRequired, string? Options, string? DefaultValue, int DisplayOrder);

public record SaveDynRecordReq(string? DisplayTitle, Dictionary<string, string?> FieldValues);
public record UpdateDynRecordReq(string? DisplayTitle, Dictionary<string, string?> FieldValues);
