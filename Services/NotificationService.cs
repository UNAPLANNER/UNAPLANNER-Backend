using UNAPLANNER_API.Infrastructure.Firebase;
using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationTokenRepository _tokenRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationSender _sender;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationTokenRepository tokenRepository,
        INotificationRepository notificationRepository,
        INotificationSender sender,
        ILogger<NotificationService> logger)
    {
        _tokenRepository = tokenRepository;
        _notificationRepository = notificationRepository;
        _sender = sender;
        _logger = logger;
    }

    public async Task SaveDeviceTokenAsync(int userId, string fcmToken, string? deviceName = null)
    {
        var existing = await _tokenRepository.GetByUserIdAndTokenAsync(userId, fcmToken);

        if (existing is not null)
        {
            existing.IsActive = true;
            existing.DeviceName = deviceName ?? existing.DeviceName;
            await _tokenRepository.UpdateAsync(existing);
        }
        else
        {
            await _tokenRepository.AddAsync(new NotificationToken
            {
                UserId = userId,
                FcmToken = fcmToken,
                DeviceName = deviceName,
                IsActive = true,
                CreatedDate = DateTime.Now,
                LastUpdated = DateTime.Now
            });
        }
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
    {
        return await _notificationRepository.GetByUserIdAsync(userId);
    }

    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        return await _notificationRepository.MarkAsReadAsync(notificationId);
    }

    public async Task<bool> MarkAllAsReadAsync(int userId)
    {
        return await _notificationRepository.MarkAllAsReadAsync(userId);
    }

    public async Task<bool> DeleteNotificationAsync(int userId, int notificationId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification is null || notification.UserId != userId)
            return false;

        return await _notificationRepository.DeleteAsync(notificationId);
    }

    public async Task<bool> DeleteAllNotificationsAsync(int userId)
    {
        return await _notificationRepository.DeleteAllAsync(userId);
    }

    public async Task SendActivityReminderAsync(
        int userId, int calendarId, string activityTitle, string activityType, DateTime activityDate)
    {
        var notifType = activityType switch
        {
            "Examen"     => "ExamReminder",
            "Tarea"      => "TaskReminder",
            "Proyecto"   => "ProjectReminder",
            "Exposicion" => "EventReminder",
            "Evento"     => "EventReminder",
            _            => "General"
        };

        var notification = new Notification
        {
            UserId = userId,
            Title = GetReminderTitle(activityType),
            Message = $"Tu actividad '{activityTitle}' está programada para el {activityDate:dd/MM/yyyy HH:mm}.",
            Type = notifType,
            RelatedId = calendarId,
            IsRead = false,
            CreatedDate = DateTime.Now,
            SentDate = DateTime.Now
        };

        await _notificationRepository.AddAsync(notification);

        var payload = NotificationPayloadFactory.ActivityReminder(calendarId, activityTitle, activityType, activityDate);
        await SendToUserAsync(userId, payload);
    }

    public async Task SendCourseApprovedAsync(int userId, string courseName)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = "Curso Aprobado",
            Message = $"¡Felicidades! Has aprobado el curso '{courseName}'.",
            Type = "CourseApproved",
            IsRead = false,
            CreatedDate = DateTime.Now,
            SentDate = DateTime.Now
        };

        await _notificationRepository.AddAsync(notification);

        var payload = NotificationPayloadFactory.CourseApproved(courseName);
        await SendToUserAsync(userId, payload);
    }

    private async Task SendToUserAsync(int userId, NotificationPayload payload)
    {
        var tokens = await _tokenRepository.GetActiveTokensByUserIdAsync(userId);

        if (tokens.Count == 0)
        {
            _logger.LogWarning("Usuario {UserId} no tiene tokens de dispositivo activos", userId);
            return;
        }

        await _sender.SendToMultipleAsync(tokens.Select(t => t.FcmToken), payload);
    }

    private static string GetReminderTitle(string activityType) => activityType switch
    {
        "Examen"     => "Recordatorio de Examen",
        "Tarea"      => "Recordatorio de Tarea",
        "Proyecto"   => "Recordatorio de Proyecto",
        "Exposicion" => "Recordatorio de Exposición",
        "Evento"     => "Recordatorio de Evento",
        _            => "Recordatorio de Actividad"
    };
}
