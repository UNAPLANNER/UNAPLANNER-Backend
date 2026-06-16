using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Route("api/student")]
[ApiController]
public class StudentCurriculumController : ControllerBase
{
    private readonly ICurriculumService _curriculumService;

    public StudentCurriculumController(ICurriculumService curriculumService)
    {
        _curriculumService = curriculumService;
    }

    /// <summary>
    /// Obtiene la malla curricular completa del estudiante organizada por año y semestre.
    /// Usa automáticamente la carrera y plan de estudios asignados al estudiante.
    /// Cada año (nivel) contiene semestre 1 y semestre 2 con sus cursos y estado de progreso.
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    [HttpGet("{id}/curriculum")]
    [ProducesResponseType(typeof(StudentCurriculumResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStudentCurriculum(int id)
    {
        try
        {
            var result = await _curriculumService.GetStudentCurriculumAsync(id);

            if (!result.Success)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Curriculum);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al obtener la malla curricular", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todos los cursos de la malla curricular del estudiante con su estado de progreso.
    /// Incluye nivel, semestre, estado (Pendiente/EnCurso/Aprobado/Reprobado) y nota final.
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    [HttpGet("{id}/curriculum/courses")]
    [ProducesResponseType(typeof(List<StudentCourseProgressResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStudentCurriculumCourses(int id)
    {
        try
        {
            var result = await _curriculumService.GetStudentCoursesWithProgressAsync(id);

            if (!result.Success)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Courses);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al obtener la malla curricular", error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza el estado de un curso en la malla curricular del estudiante.
    /// Crea el registro de progreso si no existe.
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <param name="courseId">ID del curso</param>
    /// <param name="request">Datos de actualización: estado, nota final, semestre, año</param>
    [HttpPut("{id}/courses/{courseId}")]
    [ProducesResponseType(typeof(StudentCourseProgressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCourseStatus(int id, int courseId, [FromBody] UpdateCourseStatusRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var result = await _curriculumService.UpdateCourseStatusAsync(id, courseId, request);

            if (!result.Success)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Course);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al actualizar el estado del curso", error = ex.Message });
        }
    }
}
