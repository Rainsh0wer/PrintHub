using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Notifications;
using PrintHub.Application.Features.Notifications.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>In-app notifications (UC-08). Scoped to the authenticated caller.</summary>
[ApiController]
[Route("api/notifications")]
[Authorize]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notifications;
    private readonly ICurrentUser _currentUser;

    public NotificationsController(INotificationService notifications, ICurrentUser currentUser)
    {
        _notifications = notifications;
        _currentUser = currentUser;
    }

    /// <summary>UC-08 — list own notifications (paged, newest first).</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] PageRequest page, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _notifications.ListAsync(userId.Value, page, ct)).ToActionResult();
    }

    /// <summary>UC-08 — mark notifications read (all unread when no ids are given).</summary>
    [HttpPut("read")]
    public async Task<IActionResult> MarkRead(MarkNotificationsReadRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _notifications.MarkReadAsync(userId.Value, request.Ids, ct)).ToActionResult(successMessage: "Notifications marked as read.");
    }
}
