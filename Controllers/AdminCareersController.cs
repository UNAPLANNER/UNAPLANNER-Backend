using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    private int? GetCurrentUserId()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var userId) ? userId : null;
    }
}
