using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class CalendarService : ICalendarService
{
    private readonly ICalendarRepository _calendarRepository;
    private readonly IStudentRepository _studentRepository;

    public CalendarService(ICalendarRepository calendarRepository, IStudentRepository studentRepository)
    {
        _calendarRepository = calendarRepository;
        _studentRepository = studentRepository;
    }

    public async Task<StudentCalendarResponse?> GetStudentCalendarAsync(int studentId)
    {
        // Validate that the student exists
        var student = await _studentRepository.GetStudentByIdAsync(studentId);
        if (student == null)
            return null;

        // Get all events for the student's user account
        var events = await _calendarRepository.GetEventsByUserIdAsync(student.UserId);
        
        // Build response with events and summary statistics
        return BuildCalendarResponse(events);
    }

    public async Task<CalendarEventResponse?> GetEventDetailAsync(int eventId)
    {
        var calendarEvent = await _calendarRepository.GetByIdAsync(eventId);
        if (calendarEvent == null)
            return null;

        return CalendarMapper.ToCalendarEventResponse(calendarEvent);
    }

    public async Task<StudentCalendarResponse?> GetStudentCalendarByDateRangeAsync(int studentId, DateTime startDate, DateTime endDate)
    {
        // Validate that the student exists
        var student = await _studentRepository.GetStudentByIdAsync(studentId);
        if (student == null)
            return null;

        // Get events within the specified date range
        var events = await _calendarRepository.GetEventsByDateRangeAsync(student.UserId, startDate, endDate);
        
        return BuildCalendarResponse(events);
    }

    public async Task<StudentCalendarResponse?> GetStudentCalendarByActivityTypeAsync(int studentId, string activityType)
    {
        // Validate that the student exists
        var student = await _studentRepository.GetStudentByIdAsync(studentId);
        if (student == null)
            return null;

        // Get events filtered by activity type
        var events = await _calendarRepository.GetEventsByActivityTypeAsync(student.UserId, activityType);
        
        return BuildCalendarResponse(events);
    }

    /// <summary>
    /// Builds a StudentCalendarResponse with events and summary statistics
    /// </summary>
    private static StudentCalendarResponse BuildCalendarResponse(List<Models.Entities.Calendar> events)
    {
        var eventResponses = events.Select(CalendarMapper.ToCalendarEventResponse).ToList();

        var summary = new CalendarSummaryStats
        {
            TotalEvents = events.Count,
            UpcomingEvents = events.Count(e => !e.IsCompleted && e.ActivityDate >= DateTime.Now),
            CompletedEvents = events.Count(e => e.IsCompleted),
            ExamsCount = events.Count(e => e.ActivityType.Equals("Exam", StringComparison.OrdinalIgnoreCase)),
            AssignmentsCount = events.Count(e => e.ActivityType.Equals("Assignment", StringComparison.OrdinalIgnoreCase) 
                || e.ActivityType.Equals("Project", StringComparison.OrdinalIgnoreCase))
        };

        return new StudentCalendarResponse
        {
            Events = eventResponses,
            Summary = summary
        };
    }
}
