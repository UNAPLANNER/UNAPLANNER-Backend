using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Services;

public interface INotificationService
{
    Task SaveDeviceTokenAsync(int userId, string fcmToken, string? deviceName = null);
    Task<List<Notification>> GetUserNotificationsAsync(int userId);
    Task<bool> MarkAsReadAsync(int notificationId);
    Task<bool> MarkAllAsReadAsync(int userId);
    Task<bool> DeleteNotificationAsync(int userId, int notificationId);
    Task<bool> DeleteAllNotificationsAsync(int userId);
    Task SendActivityReminderAsync(int userId, int calendarId, string activityTitle, string activityType, DateTime activityDate);
    Task SendCourseApprovedAsync(int userId, string courseName);
    Task<object> TestFcmForUserAsync(int userId);
}
