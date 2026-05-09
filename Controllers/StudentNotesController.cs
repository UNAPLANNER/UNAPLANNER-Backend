using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.Services;

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
    /// Obtiene las notas de un estudiante usando el ID de usuario devuelto por login.
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="courseId">ID del curso (opcional) para filtrar por curso</param>
    /// <returns>Lista de notas del estudiante</returns>
    [HttpGet("user/{userId}/notes")]
    public async Task<IActionResult> GetStudentNotesByUserId(int userId, [FromQuery] int? courseId = null)
    {
        try
        {
            var result = await _notesService.GetStudentNotesByUserIdAsync(userId, courseId);

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
    /// Crea una nota para el estudiante usando el ID de usuario devuelto por login.
    /// </summary>
    [HttpPost("user/{userId}/notes")]
    public async Task<IActionResult> CreateNoteByUserId(int userId, [FromBody] CreateNoteRequest request)
    {
        try
        {
            var result = await _notesService.CreateNoteByUserIdAsync(userId, request);

            if (!result.Success)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Created($"/api/notes/{result.Note!.Id}", result.Note);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al crear la nota: {ex.Message}" });
        }
    }
}
