using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface ICalendarRepository
{
    /// <summary>
    /// Gets all calendar events for a specific user, ordered by activity date
    /// </summary>
    /// <param name="userId">The user ID to retrieve events for</param>
    /// <returns>List of calendar events for the user</returns>
    Task<List<Calendar>> GetEventsByUserIdAsync(int userId);

    /// <summary>
    /// Gets a specific calendar event by its ID
    /// </summary>
    /// <param name="id">The calendar event ID</param>
    /// <returns>Calendar event if found, null otherwise</returns>
    Task<Calendar?> GetByIdAsync(int id);

    /// <summary>
    /// Validates that a course exists by its ID
    /// </summary>
    /// <param name="courseId">The course ID to validate</param>
    /// <returns>True if the course exists, false otherwise</returns>
    Task<bool> CourseExistsAsync(int courseId);

    /// <summary>
    /// Gets calendar events for a user within a specific date range
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="startDate">Start date for filtering</param>
    /// <param name="endDate">End date for filtering</param>
    /// <returns>List of calendar events within the date range</returns>
    Task<List<Calendar>> GetEventsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets calendar events by activity type for a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="activityType">Type of activity to filter by (e.g., "Exam", "Assignment")</param>
    /// <returns>List of calendar events matching the activity type</returns>
    Task<List<Calendar>> GetEventsByActivityTypeAsync(int userId, string activityType);

    /// <summary>
    /// Creates a new calendar event for a user
    /// </summary>
    /// <param name="calendarEvent">The calendar event to create</param>
    /// <returns>The created calendar event with generated ID</returns>
    Task<Calendar> CreateAsync(Calendar calendarEvent);
}
