using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Route("api/admin/careers")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminCareersController : ControllerBase
{
    private readonly ICareerService _careerService;

    public AdminCareersController(ICareerService careerService)
    {
        _careerService = careerService;
    }

    /// <summary>
    /// Retrieves the available majors for the authenticated administrator's campus.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CareerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCareersByAuthenticatedAdminCampus()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized(new { message = "No se pudo identificar el usuario autenticado." });
            }

            var careers = await _careerService.GetCareersByAdminUserIdAsync(currentUserId.Value);
            return Ok(careers);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al obtener las carreras del administrador", error = ex.Message });
        }
    }

    /// <summary>
    /// Create a session for the authenticated administrator's campus.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CareerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCareer([FromBody] CreateCareerRequest request)
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

            var career = await _careerService.CreateCareerForAdminAsync(currentUserId.Value, request);
            return CreatedAtAction(nameof(GetCareersByAuthenticatedAdminCampus), new { id = career.Id }, career);
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
                new { message = "Error al crear la carrera", error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing career for the authenticated administrator's campus.
    /// </summary>
    [HttpPut("{id}")]
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

    private int? GetCurrentUserId()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var userId) ? userId : null;
    }
}
