using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Authorize]
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

    private bool IsCurrentStudent(int studentId)
    {
        var claim = User.FindFirstValue("StudentId");
        return int.TryParse(claim, out var id) && id == studentId;
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
        if (!IsCurrentStudent(id)) return Forbid();
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
        if (!IsCurrentStudent(id)) return Forbid();
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
    /// Gets the full detail of a specific course for the student.
    /// Includes basic info, hours, prerequisites with pass status, and enrollment details if enrolled.
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <param name="courseId">Course ID</param>
    [HttpGet("{id}/courses/{courseId}/detail")]
    [ProducesResponseType(typeof(CourseDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCourseDetail(int id, int courseId)
    {
        if (!IsCurrentStudent(id)) return Forbid();
        try
        {
            var result = await _curriculumService.GetCourseDetailAsync(id, courseId);

            if (!result.Success)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Detail);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al obtener el detalle del curso", error = ex.Message });
        }
    }

    /// <summary>
    /// Creates the enrolled detail (professor, classroom, schedule, syllabus) for an in-progress course.
    /// Fails with 409 if a detail record already exists; use PUT to update it.
    /// </summary>
    [HttpPost("{id}/courses/{courseId}/enrolled-detail")]
    [ProducesResponseType(typeof(CourseDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateEnrolledDetail(int id, int courseId, [FromBody] EnrolledCourseDetailRequest request)
    {
        if (!IsCurrentStudent(id)) return Forbid();
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var result = await _curriculumService.CreateEnrolledDetailAsync(id, courseId, request);

            if (!result.Success)
            {
                if (result.IsConflict)
                    return Conflict(new { message = result.ErrorMessage });
                if (result.IsBadRequest)
                    return BadRequest(new { message = result.ErrorMessage });
                return NotFound(new { message = result.ErrorMessage });
            }

            return StatusCode(StatusCodes.Status201Created, result.Detail);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al crear el detalle del curso", error = ex.Message });
        }
    }

    /// <summary>
    /// Creates or updates the enrolled detail (professor, classroom, schedule, syllabus) for an in-progress course.
    /// Passing null values clears the corresponding field.
    /// </summary>
    [HttpPut("{id}/courses/{courseId}/enrolled-detail")]
    [ProducesResponseType(typeof(CourseDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateEnrolledDetail(int id, int courseId, [FromBody] EnrolledCourseDetailRequest request)
    {
        if (!IsCurrentStudent(id)) return Forbid();
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var result = await _curriculumService.UpdateEnrolledDetailAsync(id, courseId, request);

            if (!result.Success)
            {
                if (result.IsBadRequest)
                    return BadRequest(new { message = result.ErrorMessage });
                return NotFound(new { message = result.ErrorMessage });
            }

            return Ok(result.Detail);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al actualizar el detalle del curso", error = ex.Message });
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
        if (!IsCurrentStudent(id)) return Forbid();
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
