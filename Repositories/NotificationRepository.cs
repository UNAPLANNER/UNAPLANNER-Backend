using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;
// This class implements the INotificationRepository interface, providing methods to manage notifications in the database, including adding new notifications, retrieving notifications by user ID and notification ID, and marking notifications as read.
public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;
// The constructor initializes the repository with the application's database context, allowing it to perform database operations related to notifications.
    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }
// This method adds a new notification to the database and returns the added notification, including its generated ID.
    public async Task<Notification> AddAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }
// This method retrieves a list of notifications for a specific user, ordered by creation date in descending order.
    public async Task<List<Notification>> GetByUserIdAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }
// This method retrieves a single notification by its ID, returning null if the notification does not exist.
    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await _context.Notifications.FindAsync(id);
    }
    // This method marks a specific notification as read by setting its IsRead property to true and saving the changes to the database, returning true if the operation was successful.
    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification is null) return false;

        notification.IsRead = true;
        return await _context.SaveChangesAsync() > 0;
    }
// This method marks all notifications for a specific user as read by retrieving all unread notifications for the user, setting their IsRead property to true, and saving the changes to the database, returning true if the operation was successful.
    public async Task<bool> MarkAllAsReadAsync(int userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        if (unread.Count == 0) return true;

        foreach (var n in unread)
            n.IsRead = true;

        return await _context.SaveChangesAsync() > 0;
    }
}
