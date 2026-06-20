using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Authorize]
[Route("/api")]
[ApiController]
public class EvaluationController : ControllerBase
{
    private readonly IEvaluationService _evaluationService;

    public EvaluationController(IEvaluationService evaluationService)
    {
        _evaluationService = evaluationService;
    }

    private bool IsCurrentStudent(int studentId)
    {
        var claim = User.FindFirstValue("StudentId");
        return int.TryParse(claim, out var id) && id == studentId;
    }

    /// <summary>
    /// Get course evaluations for a student.
    /// </summary>
    [HttpGet("student/{studentId}/courses/{courseId}/evaluations")]
    public async Task<IActionResult> GetCourseEvaluations(int studentId, int courseId)
    {
        if (!IsCurrentStudent(studentId)) return Forbid();
        try
        {
            var result = await _evaluationService.GetCourseEvaluationsAsync(courseId, studentId);
            if (!result.Success)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Data!.Evaluations);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al obtener las evaluaciones: {ex.Message}" });
        }
    }

    /// <summary>
    /// Creates a new evaluation for the student's course.
    /// </summary>
    [HttpPost("student/{studentId}/courses/{courseId}/evaluations")]
    public async Task<IActionResult> CreateEvaluation(int studentId, int courseId, [FromBody] CreateEvaluationRequest request)
    {
        if (!IsCurrentStudent(studentId)) return Forbid();

        if (request == null)
            return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío." });

        try
        {
            var result = await _evaluationService.CreateEvaluationAsync(courseId, studentId, request);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(
                nameof(GetCourseEvaluations),
                new { studentId = studentId, courseId = courseId },
                result.Evaluation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al crear la evaluación: {ex.Message}" });
        }
    }

    /// <summary>
    /// Updates the name, type, percentage, score, and date of an evaluation.
    /// </summary>
    [HttpPut("student/{studentId}/courses/{courseId}/evaluations/{id}")]
    public async Task<IActionResult> UpdateEvaluation(int studentId, int courseId, int id, [FromBody] UpdateEvaluationRequest request)
    {
        if (!IsCurrentStudent(studentId)) return Forbid();

        if (request == null)
            return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío." });

        try
        {
            var result = await _evaluationService.UpdateEvaluationAsync(id, studentId, request);
            if (!result.Success)
            {
                if (!string.IsNullOrEmpty(result.ErrorMessage) && result.ErrorMessage.Contains("no encontrada"))
                    return NotFound(new { message = result.ErrorMessage });
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(result.Evaluation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al actualizar la evaluación: {ex.Message}" });
        }
    }

    /// <summary>
    /// Deletes an evaluation from the student.
    /// </summary>
    [HttpDelete("student/{studentId}/courses/{courseId}/evaluations/{id}")]
    public async Task<IActionResult> DeleteEvaluation(int studentId, int courseId, int id)
    {
        if (!IsCurrentStudent(studentId)) return Forbid();
        try
        {
            var result = await _evaluationService.DeleteEvaluationAsync(id, studentId);
            if (!result.Success)
            {
                if (!string.IsNullOrEmpty(result.ErrorMessage) && result.ErrorMessage.Contains("no encontrada"))
                    return NotFound(new { message = result.ErrorMessage });
                return BadRequest(new { message = result.ErrorMessage });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al eliminar la evaluación: {ex.Message}" });
        }
    }
}
