namespace UNAPLANNER_API.DTOs.Responses;

public class CalendarEventResponse
{
    /// <summary>
    /// Unique identifier of the calendar event
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Title of the event (e.g., "Exam - Data Structures")
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the event
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Date and time when the event occurs
    /// </summary>
    public DateTime ActivityDate { get; set; }

    /// <summary>
    /// Type of activity: "Exam", "Assignment", "Project", etc.
    /// </summary>
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Course ID associated with the event (nullable)
    /// </summary>
    public int? CourseId { get; set; }

    /// <summary>
    /// Course name associated with the event (nullable)
    /// </summary>
    public string? CourseName { get; set; }

    /// <summary>
    /// Indicates if a reminder is set for this event
    /// </summary>
    public bool HasReminder { get; set; }

    /// <summary>
    /// Date and time when the reminder is triggered (nullable)
    /// </summary>
    public DateTime? ReminderDate { get; set; }

    /// <summary>
    /// Indicates if the event has been completed
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Date and time when the event was created
    /// </summary>
    public DateTime CreatedDate { get; set; }
}
