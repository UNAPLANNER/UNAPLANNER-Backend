namespace UNAPLANNER_API.DTOs.Responses;

public class StudentCalendarResponse
{
    /// <summary>
    /// List of calendar events for the student, organized chronologically
    /// </summary>
    public List<CalendarEventResponse> Events { get; set; } = new List<CalendarEventResponse>();

    /// <summary>
    /// Summary statistics about the student's calendar
    /// </summary>
    public CalendarSummaryStats Summary { get; set; } = new CalendarSummaryStats();
}

public class CalendarSummaryStats
{
    /// <summary>
    /// Total number of events in the student's calendar
    /// </summary>
    public int TotalEvents { get; set; }

    /// <summary>
    /// Number of upcoming events (not yet completed)
    /// </summary>
    public int UpcomingEvents { get; set; }

    /// <summary>
    /// Number of completed events
    /// </summary>
    public int CompletedEvents { get; set; }

    /// <summary>
    /// Count of exams scheduled
    /// </summary>
    public int ExamsCount { get; set; }

    /// <summary>
    /// Count of assignments/tasks scheduled
    /// </summary>
    public int AssignmentsCount { get; set; }
}
