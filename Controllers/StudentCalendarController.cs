using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Route("api/student")]
[ApiController]
public class StudentCalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;

    public StudentCalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    /// <summary>
    /// Gets all calendar events for a student, including exams and assignments.
    /// Events are displayed with icons based on type and can be filtered by date.
    /// </summary>
    /// <param name="id">The student ID</param>
    [HttpGet("{id}/calendar")]
    [ProducesResponseType(typeof(StudentCalendarResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStudentCalendar(int id)
    {
        try
        {
            var calendar = await _calendarService.GetStudentCalendarAsync(id);

            if (calendar == null)
                return NotFound(new { message = $"Estudiante con ID {id} no encontrado" });

            return Ok(calendar);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Error al obtener el calendario del estudiante", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets the details of a specific calendar event.
    /// Returns full information about the event including course, reminder, and completion status.
    /// </summary>
    /// <param name="id">The student ID</param>
    /// <param name="eventId">The calendar event ID</param>
    [HttpGet("{id}/calendar/{eventId}")]
    [ProducesResponseType(typeof(CalendarEventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEventDetail(int id, int eventId)
    {
        try
        {
            // Verify the student exists
            var calendar = await _calendarService.GetStudentCalendarAsync(id);
            if (calendar == null)
                return NotFound(new { message = $"Estudiante con ID {id} no encontrado" });

            // Get the specific event details
            var eventDetail = await _calendarService.GetEventDetailAsync(eventId);

            if (eventDetail == null)
                return NotFound(new { message = $"Evento del calendario con ID {eventId} no encontrado" });

            return Ok(eventDetail);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Error al obtener los detalles del evento", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets calendar events for a student within a specific date range.
    /// Useful for filtering events by week, month, or custom date ranges.
    /// </summary>
    /// <param name="id">The student ID</param>
    /// <param name="startDate">Start date for filtering (ISO 8601 format: yyyy-MM-dd)</param>
    /// <param name="endDate">End date for filtering (ISO 8601 format: yyyy-MM-dd)</param>
    [HttpGet("{id}/calendar/filter/date-range")]
    [ProducesResponseType(typeof(StudentCalendarResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCalendarByDateRange(int id, 
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            // Validate date range
            if (startDate > endDate)
                return BadRequest(new { message = "La fecha de inicio debe ser anterior a la fecha final" });

            var calendar = await _calendarService.GetStudentCalendarByDateRangeAsync(id, startDate, endDate);

            if (calendar == null)
                return NotFound(new { message = $"Estudiante con ID {id} no encontrado" });

            return Ok(calendar);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Error al obtener el calendario del estudiante", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets calendar events filtered by activity type (Exam, Assignment, Project, etc.).
    /// Allows students to view specific types of events.
    /// </summary>
    /// <param name="id">The student ID</param>
    /// <param name="activityType">Type of activity to filter by (e.g., "Exam", "Assignment", "Project")</param>
    [HttpGet("{id}/calendar/filter/activity-type")]
    [ProducesResponseType(typeof(StudentCalendarResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCalendarByActivityType(int id, 
        [FromQuery] string activityType)
    {
        try
        {
            // Validate activity type parameter
            if (string.IsNullOrWhiteSpace(activityType))
                return BadRequest(new { message = "El tipo de actividad es requerido" });

            var calendar = await _calendarService.GetStudentCalendarByActivityTypeAsync(id, activityType);

            if (calendar == null)
                return NotFound(new { message = $"Estudiante con ID {id} no encontrado" });

            return Ok(calendar);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Error al obtener el calendario del estudiante", error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new calendar event for a student.
    /// Validates the event data and generates the event ID automatically.
    /// </summary>
    /// <param name="id">The student ID</param>
    /// <param name="request">The create event request containing event information</param>
    [HttpPost("{id}/calendar")]
    [ProducesResponseType(typeof(CalendarEventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCalendarEvent(int id, [FromBody] CreateCalendarEventRequest request)
    {
        // Validate the request body
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var createdEvent = await _calendarService.CreateCalendarEventAsync(id, request);

            if (createdEvent == null)
                return NotFound(new { message = $"Estudiante con ID {id} no encontrado" });

            return CreatedAtAction(nameof(GetEventDetail), new { id, eventId = createdEvent.Id }, createdEvent);
        }
        catch (InvalidOperationException ex)
        {
            // Handle validation errors like past dates or invalid course
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Get the inner exception details for database errors
            var innerError = ex.InnerException?.Message ?? ex.Message;
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al crear el evento del calendario", error = innerError });
        }
    }

    /// <summary>
    /// Updates an existing calendar event for a student.
    /// </summary>
    /// <param name="id">The student ID</param>
    /// <param name="eventId">The calendar event ID to update</param>
    /// <param name="request">The update event request with new data</param>
    [HttpPut("{id}/calendar/{eventId}")]
    [ProducesResponseType(typeof(CalendarEventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCalendarEvent(int id, int eventId, [FromBody] UpdateCalendarEventRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var updatedEvent = await _calendarService.UpdateCalendarEventAsync(id, eventId, request);

            if (updatedEvent == null)
                return NotFound(new { message = $"Estudiante con ID {id} no encontrado" });

            return Ok(updatedEvent);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al actualizar el evento del calendario", error = innerError });
        }
    }

    /// <summary>
    /// Deletes a calendar event belonging to a student.
    /// </summary>
    /// <param name="id">The student ID</param>
    /// <param name="eventId">The calendar event ID to delete</param>
    [HttpDelete("{id}/calendar/{eventId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCalendarEvent(int id, int eventId)
    {
        try
        {
            var result = await _calendarService.DeleteCalendarEventAsync(id, eventId);

            if (result == null)
                return NotFound(new { message = $"Estudiante con ID {id} no encontrado" });

            if (result == false)
                return NotFound(new { message = $"Evento del calendario con ID {eventId} no encontrado" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al eliminar el evento del calendario", error = ex.Message });
        }
    }
}
