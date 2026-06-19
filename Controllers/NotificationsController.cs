using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Route("api/users/{userId}/notifications")]
[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Register or update the user's device FCM token to receive push notifications.
    /// </summary>
    [HttpPost("device-token")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterDeviceToken(int userId, [FromBody] RegisterDeviceTokenRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            await _notificationService.SaveDeviceTokenAsync(userId, request.FcmToken, request.DeviceName);
            return Ok(new { message = "Token de dispositivo registrado correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al registrar el token del dispositivo", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets all notifications for the user, ordered from most recent to oldest.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetNotifications(int userId)
    {
        try
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);

            var response = notifications.Select(n => new NotificationResponse
            {
                Id          = n.Id,
                Title       = n.Title,
                Message     = n.Message,
                Type        = n.Type,
                RelatedId   = n.RelatedId,
                IsRead      = n.IsRead,
                CreatedDate = n.CreatedDate,
                SentDate    = n.SentDate
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al obtener las notificaciones", error = ex.Message });
        }
    }

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    [HttpPatch("{notificationId}/read")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkAsRead(int userId, int notificationId)
    {
        try
        {
            var result = await _notificationService.MarkAsReadAsync(notificationId);

            if (!result)
                return NotFound(new { message = $"Notificación con ID {notificationId} no encontrada" });

            return Ok(new { message = "Notificación marcada como leída" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al marcar la notificación como leída", error = ex.Message });
        }
    }

    /// <summary>
    /// Marks all notifications for the user as read.
    /// </summary>
    [HttpPatch("read-all")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkAllAsRead(int userId)
    {
        try
        {
            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { message = "Todas las notificaciones marcadas como leídas" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al marcar las notificaciones como leídas", error = ex.Message });
        }
    }
}
