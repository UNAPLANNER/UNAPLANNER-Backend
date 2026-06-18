using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.DTOs.Requests;

namespace UNAPLANNER_API.Services;

public interface ICalendarService
{
    /// <summary>
    /// Gets all calendar events for a specific student with summary statistics
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <returns>Student calendar with events and summary statistics</returns>
    Task<StudentCalendarResponse?> GetStudentCalendarAsync(int studentId);

    /// <summary>
    /// Gets details of a specific calendar event
    /// </summary>
    /// <param name="eventId">The calendar event ID</param>
    /// <returns>Calendar event details if found, null otherwise</returns>
    Task<CalendarEventResponse?> GetEventDetailAsync(int eventId);

    /// <summary>
    /// Gets calendar events for a student within a specific date range
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <param name="startDate">Start date for filtering</param>
    /// <param name="endDate">End date for filtering</param>
    /// <returns>Student calendar with filtered events</returns>
    Task<StudentCalendarResponse?> GetStudentCalendarByDateRangeAsync(int studentId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets calendar events filtered by activity type for a specific student
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <param name="activityType">Type of activity to filter by (e.g., "Exam", "Assignment")</param>
    /// <returns>Student calendar with filtered events by activity type</returns>
    Task<StudentCalendarResponse?> GetStudentCalendarByActivityTypeAsync(int studentId, string activityType);

    /// <summary>
    /// Creates a new calendar event for a student
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <param name="request">The create event request with event data</param>
    /// <returns>The created calendar event response with generated ID</returns>
    Task<CalendarEventResponse?> CreateCalendarEventAsync(int studentId, CreateCalendarEventRequest request);

    /// <summary>
    /// Updates an existing calendar event for a student
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <param name="eventId">The calendar event ID to update</param>
    /// <param name="request">The update event request with new data</param>
    /// <returns>The updated event, null if student not found, throws KeyNotFoundException if event not found</returns>
    Task<CalendarEventResponse?> UpdateCalendarEventAsync(int studentId, int eventId, UpdateCalendarEventRequest request);

    /// <summary>
    /// Deletes a calendar event belonging to a student
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <param name="eventId">The calendar event ID to delete</param>
    /// <returns>True if deleted, false if event not found, null if student not found</returns>
    Task<bool?> DeleteCalendarEventAsync(int studentId, int eventId);
}
