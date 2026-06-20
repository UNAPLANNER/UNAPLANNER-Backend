using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Route("api/study-plans")]
[ApiController]
[Authorize(Roles = "Admin")]
public class StudyPlansController : ControllerBase
{
    private readonly IStudyPlanService _studyPlanService;

    public StudyPlansController(IStudyPlanService studyPlanService)
    {
        _studyPlanService = studyPlanService;
    }

    /// <summary>
    /// Gets a study plan detail organized by level and semester.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudyPlanDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStudyPlanDetail(int id)
    {
        try
        {
            var studyPlan = await _studyPlanService.GetStudyPlanDetailAsync(id);
            return Ok(studyPlan);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al obtener el plan de estudios", error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a study plan.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(StudyPlanDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateStudyPlan([FromBody] CreateStudyPlanRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var studyPlan = await _studyPlanService.CreateStudyPlanAsync(request);
            return CreatedAtAction(nameof(GetStudyPlanDetail), new { id = studyPlan.Id }, studyPlan);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al crear el plan de estudios", error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a course and assigns it to a study plan.
    /// </summary>
    [HttpPost("{id}/courses")]
    [ProducesResponseType(typeof(StudyPlanDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateStudyPlanCourse(
        int id,
        [FromBody] CreateStudyPlanCourseRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var studyPlan = await _studyPlanService.CreateStudyPlanCourseAsync(id, request);
            return CreatedAtAction(nameof(GetStudyPlanDetail), new { id = studyPlan.Id }, studyPlan);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al crear el curso", error = ex.Message });
        }
    }

    /// <summary>
    /// Updates a course assigned to a study plan.
    /// </summary>
    [HttpPut("{id}/courses/{courseId}")]
    [ProducesResponseType(typeof(StudyPlanDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateStudyPlanCourse(
        int id,
        int courseId,
        [FromBody] UpdateStudyPlanCourseRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var studyPlan = await _studyPlanService.UpdateStudyPlanCourseAsync(id, courseId, request);
            return Ok(studyPlan);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al actualizar el curso", error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a course from a study plan.
    /// </summary>
    [HttpDelete("{id}/courses/{courseId}")]
    [ProducesResponseType(typeof(StudyPlanDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteStudyPlanCourse(int id, int courseId)
    {
        try
        {
            var studyPlan = await _studyPlanService.DeleteStudyPlanCourseAsync(id, courseId);
            return Ok(studyPlan);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al eliminar el curso", error = ex.Message });
        }
    }
}
