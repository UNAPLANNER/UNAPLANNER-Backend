using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Mappers;

public class CalendarMapper
{
    /// <summary>
    /// Maps a Calendar entity to a CalendarEventResponse DTO
    /// </summary>
    /// <param name="calendar">The calendar entity to map</param>
    /// <returns>CalendarEventResponse with populated data</returns>
    public static CalendarEventResponse ToCalendarEventResponse(Calendar calendar)
    {
        return new CalendarEventResponse
        {
            Id = calendar.Id,
            Title = calendar.Title,
            Description = calendar.Description,
            ActivityDate = calendar.ActivityDate,
            ActivityType = calendar.ActivityType,
            CourseId = calendar.CourseId,
            CourseName = calendar.Course?.Name ?? null,
            HasReminder = calendar.HasReminder,
            ReminderDate = calendar.ReminderDate,
            IsCompleted = calendar.IsCompleted,
            CreatedDate = calendar.CreatedDate
        };
    }

    /// <summary>
    /// Maps a list of Calendar entities to a list of CalendarEventResponse DTOs
    /// </summary>
    /// <param name="calendars">The list of calendar entities to map</param>
    /// <returns>List of CalendarEventResponse with populated data</returns>
    public static List<CalendarEventResponse> ToCalendarEventResponseList(List<Calendar> calendars)
    {
        return calendars.Select(ToCalendarEventResponse).ToList();
    }

    /// <summary>
    /// Maps a CreateCalendarEventRequest DTO to a Calendar entity
    /// Generates the created date and sets default values for new events
    /// </summary>
    /// <param name="request">The create event request</param>
    /// <param name="userId">The user ID for the event owner</param>
    /// <returns>Calendar entity ready to be persisted</returns>
    public static Calendar ToEntity(CreateCalendarEventRequest request, int userId)
    {
        return new Calendar
        {
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            ActivityDate = request.ActivityDate,
            ActivityType = request.ActivityType,
            CourseId = request.CourseId,
            HasReminder = request.HasReminder,
            ReminderDate = request.ReminderDate,
            IsCompleted = false,
            CreatedDate = DateTime.Now
        };
    }

    public static void ApplyUpdate(Calendar entity, UpdateCalendarEventRequest request)
    {
        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.ActivityDate = request.ActivityDate;
        entity.ActivityType = request.ActivityType;
        entity.CourseId = request.CourseId;
        entity.HasReminder = request.HasReminder;
        entity.ReminderDate = request.ReminderDate;
        entity.IsCompleted = request.IsCompleted;
    }
}
