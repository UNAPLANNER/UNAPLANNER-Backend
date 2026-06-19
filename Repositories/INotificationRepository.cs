using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface INotificationRepository
{
    Task<Notification> AddAsync(Notification notification);
    Task<List<Notification>> GetByUserIdAsync(int userId);
    Task<Notification?> GetByIdAsync(int id);
    Task<bool> MarkAsReadAsync(int notificationId);
    Task<bool> MarkAllAsReadAsync(int userId);
}
