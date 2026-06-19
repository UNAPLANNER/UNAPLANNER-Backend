using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
}
