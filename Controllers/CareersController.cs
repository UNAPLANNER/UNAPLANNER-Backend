using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Route("api/careers")]
[ApiController]
public class CareersController : ControllerBase
{
    private readonly ICareerService _careerService;

    public CareersController(ICareerService careerService)
    {
        _careerService = careerService;
    }

    /// <summary>
    /// Get a list of all available majors.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CareerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllCareers()
    {
        try
        {
            var careers = await _careerService.GetAllCareersAsync();
            return Ok(careers);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al obtener las carreras", error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing career for the authenticated administrator's campus.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CareerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCareer(int id, [FromBody] UpdateCareerRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized(new { message = "No se pudo identificar el usuario autenticado." });
            }

            var career = await _careerService.UpdateCareerForAdminAsync(currentUserId.Value, id, request);
            return Ok(career);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al actualizar la carrera", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene la malla curricular de una carrera organizada por nivel y semestre.
    /// Si se proporciona userId, incluye el progreso del estudiante.
    /// </summary>
    /// <param name="id">ID de la carrera</param>
    /// <param name="userId">ID del usuario (opcional) para incluir progreso del estudiante</param>
    [HttpGet("{id}/curriculum")]
    [ProducesResponseType(typeof(List<LevelCurriculumResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurriculum(int id, [FromQuery] int? userId = null)
    {
        try
        {
            var curriculum = await _careerService.GetCurriculumByCareerIdAsync(id, userId);
            return Ok(curriculum);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al obtener la malla curricular", error = ex.Message });
        }
    }

    private int? GetCurrentUserId()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var userId) ? userId : null;
    }
}
