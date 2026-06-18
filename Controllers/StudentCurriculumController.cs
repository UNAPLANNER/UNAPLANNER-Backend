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
    private readonly ILogger<StudentCurriculumController> _logger;

    public StudentCurriculumController(ICurriculumService curriculumService, ILogger<StudentCurriculumController> logger)
    {
        _curriculumService = curriculumService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the student's complete resume organized by year and semester.
    /// Automatically uses the major and study plan assigned to the student.
    /// Each year (level) contains semester 1 and semester 2 with their courses and progress status.
    /// </summary>
    /// <param name="id">Student ID</param>
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
    /// Gets all courses in the student's curriculum with their progress status.
    /// Includes level, semester, status (Pending/In Progress/Passed/Failed) and final grade.
    /// </summary>
    /// <param name="id">Student ID</param>
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
    /// Updates the status of a course in the student's curriculum.
    /// Creates a progress record if it doesn't exist.
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <param name="courseId">Course ID</param>
    /// <param name="request">Update data: status, final grade, semester, year</param>
    [HttpPut("{id}/courses/{courseId}")]
    [ProducesResponseType(typeof(StudentCourseProgressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCourseStatus(int id, int courseId, [FromBody] UpdateCourseStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("PUT student/{Id}/courses/{CourseId} — validación fallida: {Errors}",
                id, courseId, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return ValidationProblem(ModelState);
        }

        try
        {
            var result = await _curriculumService.UpdateCourseStatusAsync(id, courseId, request);

            if (!result.Success)
            {
                _logger.LogWarning("PUT student/{Id}/courses/{CourseId} — {Error}", id, courseId, result.ErrorMessage);
                if (result.IsBadRequest)
                    return BadRequest(new { message = result.ErrorMessage });
                return NotFound(new { message = result.ErrorMessage });
            }

            return Ok(result.Course);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PUT student/{Id}/courses/{CourseId} — excepción", id, courseId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al actualizar el estado del curso", error = ex.Message });
        }
    }
}
