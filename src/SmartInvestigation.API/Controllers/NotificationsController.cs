using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInvestigation.Application.Features.Admin;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    public NotificationsController(IMediator mediator) => _mediator = mediator;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] bool unreadOnly = false,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetUserNotificationsQuery(UserId, unreadOnly, pageNumber, pageSize));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _mediator.Send(new MarkNotificationReadCommand(id, UserId));
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
    }
}
