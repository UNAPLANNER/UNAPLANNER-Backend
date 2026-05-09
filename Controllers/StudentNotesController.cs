using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.Services;
using UNAPLANNER_API.DTOs.Requests;

namespace UNAPLANNER_API.Controllers;

[Route("/api")]
[ApiController]
public class StudentNotesController : ControllerBase
{
    private readonly INotesService _notesService;

    public StudentNotesController(INotesService notesService)
    {
        _notesService = notesService;
    }

    /// <summary>
    /// Obtiene las notas de un estudiante
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <param name="courseId">ID del curso (opcional) para filtrar por curso</param>
    /// <returns>Lista de notas del estudiante</returns>
    [HttpGet("student/{id}/notes")]
    public async Task<IActionResult> GetStudentNotes(int id, [FromQuery] int? courseId = null)
    {
        try
        {
            var result = await _notesService.GetStudentNotesAsync(id, courseId);

            if (!result.Success)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { data = result.Notes, message = "Notas obtenidas exitosamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al obtener las notas: {ex.Message}" });
        }
    }

    /// <summary>
    /// Crea una nueva nota para un estudiante
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <param name="request">Datos de la nota a crear</param>
    /// <returns>Nota creada</returns>
    [HttpPost("student/{id}/notes")]
    public async Task<IActionResult> CreateStudentNote(int id, [FromBody] CreateNoteRequest request)
    {
        try
        {
            // Validar que el request no sea nulo
            if (request == null)
            {
                return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío." });
            }

            // Llamar al servicio para crear la nota
            var result = await _notesService.CreateNoteAsync(id, request);

            if (!result.Success)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return CreatedAtAction(nameof(GetStudentNotes), new { id = id }, new { data = result.Note, message = "Nota creada exitosamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al crear la nota: {ex.Message}" });
        }
    }

    /// <summary>
    /// Actualiza una nota existente
    /// </summary>
    /// <param name="id">ID de la nota a actualizar</param>
    /// <param name="request">Datos actualizados de la nota</param>
    /// <returns>Nota actualizada</returns>
    [HttpPut("notes/{id}")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteRequest request)
    {
        try
        {
            // Validar que el request no sea nulo
            if (request == null)
            {
                return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío." });
            }

            // Llamar al servicio para actualizar la nota
            var result = await _notesService.UpdateNoteAsync(id, request);

            if (!result.Success)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { data = result.Note, message = "Nota actualizada exitosamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al actualizar la nota: {ex.Message}" });
        }
    }

    /// <summary>
    /// Obtiene todos los cursos del plan de estudios de un estudiante
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <returns>Lista de cursos disponibles para el estudiante</returns>
    [HttpGet("student/{id}/courses")]
    public async Task<IActionResult> GetStudentCourses(int id)
    {
        try
        {
            var result = await _notesService.GetStudentCoursesAsync(id);

            if (!result.Success)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { data = result.Courses, message = "Cursos obtenidos exitosamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al obtener los cursos: {ex.Message}" });
        }
    }

    /// <summary>
    /// Elimina una nota existente
    /// </summary>
    /// <param name="id">ID de la nota a eliminar</param>
    /// <param name="userId">ID del usuario propietario de la nota (para validación de propiedad)</param>
    /// <returns>204 No Content si se elimina exitosamente</returns>
    [HttpDelete("notes/{id}")]
    public async Task<IActionResult> DeleteNote(int id, [FromQuery] int userId)
    {
        try
        {
            // Validar que el userId está presente
            if (userId <= 0)
            {
                return BadRequest(new { message = "El ID del usuario es requerido." });
            }

            // Llamar al servicio para eliminar la nota
            var result = await _notesService.DeleteNoteAsync(id, userId);

            if (!result.Success)
            {
                // Si es "No encontrada" retornar 404, si es "Sin permisos" retornar 403
                if (!string.IsNullOrEmpty(result.ErrorMessage) && result.ErrorMessage.Contains("no encontrada"))
                {
                    return NotFound(new { message = result.ErrorMessage });
                }
                if (!string.IsNullOrEmpty(result.ErrorMessage) && result.ErrorMessage.Contains("No tienes permisos"))
                {
                    return Forbid();
                }
                return BadRequest(new { message = result.ErrorMessage });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al eliminar la nota: {ex.Message}" });
        }
    }
}
