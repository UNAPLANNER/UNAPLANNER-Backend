using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.Services;
using UNAPLANNER_API.DTOs.Requests;

namespace UNAPLANNER_API.Controllers;

[Route("/api/student")]
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
    [HttpGet("{id}/notes")]
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
    [HttpPost("{id}/notes")]
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
}
