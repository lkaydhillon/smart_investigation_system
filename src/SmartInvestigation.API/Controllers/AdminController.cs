using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.Features.Admin;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminController(IMediator mediator) => _mediator = mediator;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    // ── Lookups ──

    [HttpGet("lookups/{category}")]
    public async Task<IActionResult> GetLookups(string category)
    {
        var result = await _mediator.Send(new GetLookupsByCategoryQuery(category));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("lookups")]
    public async Task<IActionResult> CreateLookup([FromBody] CreateLookupReq request)
    {
        var result = await _mediator.Send(new CreateLookupValueCommand(request.Category, request.Code,
            request.DisplayName, request.ParentCode, request.SortOrder, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPut("lookups/{id:guid}")]
    public async Task<IActionResult> UpdateLookup(Guid id, [FromBody] UpdateLookupReq request)
    {
        var result = await _mediator.Send(new UpdateLookupValueCommand(id, request.DisplayName, request.SortOrder, request.IsActive, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── Crime Types ──

    [HttpGet("crime-types")]
    public async Task<IActionResult> GetCrimeTypes([FromQuery] bool activeOnly = true)
    {
        var result = await _mediator.Send(new GetCrimeTypesQuery(activeOnly));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("crime-types")]
    public async Task<IActionResult> CreateCrimeType([FromBody] CreateCrimeTypeReq request)
    {
        var result = await _mediator.Send(new CreateCrimeTypeCommand(request.Name, request.Code,
            request.ParentId, request.Severity, request.Description, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── Custom Fields ──

    [HttpGet("custom-fields/{entityType}")]
    public async Task<IActionResult> GetCustomFields(string entityType)
    {
        var result = await _mediator.Send(new GetCustomFieldsByEntityQuery(entityType));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("custom-fields")]
    public async Task<IActionResult> CreateCustomField([FromBody] CreateCustomFieldReq request)
    {
        var result = await _mediator.Send(new CreateCustomFieldCommand(request.EntityType, request.FieldName,
            request.DisplayLabel, request.FieldType, request.Options, request.IsRequired,
            request.DisplayOrder, request.ValidationRegex, request.GroupName, request.IsSearchable, UserId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    // ── System Config ──

    [HttpGet("config")]
    public async Task<IActionResult> GetConfigs([FromQuery] string? category = null)
    {
        var result = await _mediator.Send(new GetSystemConfigsQuery(category));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPut("config")]
    public async Task<IActionResult> UpdateConfig([FromBody] UpdateConfigReq request)
    {
        var result = await _mediator.Send(new UpdateSystemConfigCommand(request.Key, request.Value, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}

public record CreateLookupReq(string Category, string Code, string DisplayName, string? ParentCode, int SortOrder);
public record UpdateLookupReq(string DisplayName, int SortOrder, bool IsActive);
public record CreateCrimeTypeReq(string Name, string Code, Guid? ParentId, string? Severity, string? Description);
public record CreateCustomFieldReq(string EntityType, string FieldName, string DisplayLabel, CustomFieldType FieldType,
    string? Options, bool IsRequired, int DisplayOrder, string? ValidationRegex, string? GroupName, bool IsSearchable);
public record UpdateConfigReq(string Key, string? Value);
