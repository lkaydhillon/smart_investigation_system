using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.Features.Users;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin,StationHouseOfficer")]
    public async Task<IActionResult> GetUsers([FromQuery] string? rank, [FromQuery] Guid? policeStationId)
    {
        var result = await _mediator.Send(new GetUsersQuery(rank, policeStationId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserReq request)
    {
        var result = await _mediator.Send(new CreateUserCommand(request.Username, request.Email,
            request.FullName, request.Rank, request.BadgeNumber, request.RoleId));
        return result.IsSuccess ? StatusCode(201, new { id = result.Data }) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpGet("roles")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetRoles()
    {
        var result = await _mediator.Send(new GetRolesQuery());
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}

public record CreateUserReq(string Username, string Email, string FullName, string Rank, string BadgeNumber, string RoleId);
