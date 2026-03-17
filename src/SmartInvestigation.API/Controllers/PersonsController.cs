using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.Features.People;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PersonsController(IMediator mediator) => _mediator = mediator;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPersonByIdQuery(id));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new SearchPersonsQuery(searchTerm, pageNumber, pageSize));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonReq request)
    {
        var result = await _mediator.Send(new CreatePersonCommand(request.FullName, request.FatherName,
            request.Gender, request.DateOfBirth, request.Nationality, request.AadhaarHash,
            request.IdentificationMarks, UserId));
        return result.IsSuccess ? StatusCode(201, result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("{personId:guid}/link-case")]
    public async Task<IActionResult> LinkToCase(Guid personId, [FromBody] LinkPersonCaseReq request)
    {
        var result = await _mediator.Send(new LinkPersonToCaseCommand(request.CaseId, personId,
            request.Role, request.IsMainAccused, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("{personId:guid}/addresses")]
    public async Task<IActionResult> AddAddress(Guid personId, [FromBody] AddAddressReq request)
    {
        var result = await _mediator.Send(new AddPersonAddressCommand(personId, request.AddressType,
            request.AddressLine1, request.City, request.State, request.PinCode, request.IsCurrent, UserId));
        return result.IsSuccess ? StatusCode(201, result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost("{personId:guid}/contacts")]
    public async Task<IActionResult> AddContact(Guid personId, [FromBody] AddContactReq request)
    {
        var result = await _mediator.Send(new AddPersonContactCommand(personId, request.ContactType,
            request.Value, request.IsPrimary, UserId));
        return result.IsSuccess ? StatusCode(201, result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPut("case-person/{casePersonId:guid}/status")]
    public async Task<IActionResult> UpdateCasePersonStatus(Guid casePersonId, [FromBody] UpdatePersonStatusReq request)
    {
        var result = await _mediator.Send(new UpdatePersonStatusInCaseCommand(casePersonId,
            request.Status, request.ArrestDate, request.Remarks, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreatePersonReq request)
    {
        var result = await _mediator.Send(new UpdatePersonCommand(id, request.FullName, request.FatherName,
            request.Gender, request.DateOfBirth, request.Nationality, request.AadhaarHash,
            request.IdentificationMarks, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeletePersonCommand(id, UserId));
        return result.IsSuccess ? Ok(new { success = true }) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}

public record CreatePersonReq(string FullName, string? FatherName, Gender? Gender,
    DateTime? DateOfBirth, string? Nationality, string? AadhaarHash, string? IdentificationMarks);

public record LinkPersonCaseReq(Guid CaseId, PersonRole Role, bool IsMainAccused);

public record AddAddressReq(string AddressType, string? AddressLine1, string? City,
    string? State, string? PinCode, bool IsCurrent);

public record AddContactReq(string ContactType, string Value, bool IsPrimary);

public record UpdatePersonStatusReq(PersonStatus Status, DateTime? ArrestDate, string? Remarks);
