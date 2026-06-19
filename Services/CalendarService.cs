using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class CalendarService : ICalendarService
{
    private readonly ICalendarRepository _calendarRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;

    public CalendarService(
        ICalendarRepository calendarRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService)
    {
        _calendarRepository = calendarRepository;
        _studentRepository = studentRepository;
        _notificationService = notificationService;
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
// This method retrieves calendar events for a student filtered by a specific activity type (e.g., "Exam", "Assignment"), validating that the student exists and then querying the calendar repository for events matching the criteria, returning a response with the filtered events and summary statistics.
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
    /// Creates a new calendar event for a student
    /// Validates that the student exists and optionally validates the course if provided
    /// </summary>
    public async Task<CalendarEventResponse?> CreateCalendarEventAsync(int studentId, CreateCalendarEventRequest request)
    {
        // Validate that the student exists
        var student = await _studentRepository.GetStudentByIdAsync(studentId);
        if (student == null)
            return null;

        // Validate activity date is not in the past
        if (request.ActivityDate < DateTime.Now)
            throw new InvalidOperationException("La fecha del evento no puede ser anterior a la fecha actual");

        // Validate that the course exists if provided
        if (request.CourseId.HasValue && request.CourseId > 0)
        {
            var courseExists = await _calendarRepository.CourseExistsAsync(request.CourseId.Value);
            if (!courseExists)
                throw new InvalidOperationException($"El curso con ID {request.CourseId} no existe");
        }

        try
        {
            // Create the calendar entity from the request
            var calendarEvent = CalendarMapper.ToEntity(request, student.UserId);
            
            // Save to database
            var createdEvent = await _calendarRepository.CreateAsync(calendarEvent);

            // If the event has a reminder, send a notification to the student
            if (createdEvent.HasReminder)
            {
                var reminderDate = createdEvent.ReminderDate ?? createdEvent.ActivityDate;
                await _notificationService.SendActivityReminderAsync(
                    student.UserId,
                    createdEvent.Id,
                    createdEvent.Title,
                    createdEvent.ActivityType,
                    reminderDate);
            }

            // Return the response with the generated ID
            return CalendarMapper.ToCalendarEventResponse(createdEvent);
        }
        catch (Exception ex)
        {
            // Re-throw with more context
            throw new InvalidOperationException($"Error al guardar el evento: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }
// This method updates an existing calendar event for a student, validating that the student and event exist, ensuring the event belongs to the student, optionally validating the course if provided, applying the updates from the request, saving the changes to the database, and returning the updated event details in the response.
    public async Task<CalendarEventResponse?> UpdateCalendarEventAsync(int studentId, int eventId, UpdateCalendarEventRequest request)
    {
        var student = await _studentRepository.GetStudentByIdAsync(studentId);
        if (student == null)
            return null;

        var calendarEvent = await _calendarRepository.GetByIdAsync(eventId);
        if (calendarEvent == null || calendarEvent.UserId != student.UserId)
            throw new KeyNotFoundException($"Evento del calendario con ID {eventId} no encontrado");

        if (request.CourseId.HasValue && request.CourseId > 0)
        {
            var courseExists = await _calendarRepository.CourseExistsAsync(request.CourseId.Value);
            if (!courseExists)
                throw new InvalidOperationException($"El curso con ID {request.CourseId} no existe");
        }

        CalendarMapper.ApplyUpdate(calendarEvent, request);
        var updatedEvent = await _calendarRepository.UpdateAsync(calendarEvent);

        if (updatedEvent.HasReminder)
        {
            var reminderDate = updatedEvent.ReminderDate ?? updatedEvent.ActivityDate;
            await _notificationService.SendActivityReminderAsync(
                student.UserId,
                updatedEvent.Id,
                updatedEvent.Title,
                updatedEvent.ActivityType,
                reminderDate);
        }

        return CalendarMapper.ToCalendarEventResponse(updatedEvent);
    }
// This method deletes a calendar event for a student, validating that the student and event exist, ensuring the event belongs to the student, and then deleting the event from the database, returning true if the deletion was successful, false if the event was not found or did not belong to the student, and null if the student does not exist.
    public async Task<bool?> DeleteCalendarEventAsync(int studentId, int eventId)
    {
        var student = await _studentRepository.GetStudentByIdAsync(studentId);
        if (student == null)
            return null;

        var calendarEvent = await _calendarRepository.GetByIdAsync(eventId);
        if (calendarEvent == null || calendarEvent.UserId != student.UserId)
            return false;

        return await _calendarRepository.DeleteAsync(eventId);
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
