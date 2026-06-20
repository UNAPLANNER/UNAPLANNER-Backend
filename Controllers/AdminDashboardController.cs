using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Route("api/admin/dashboard")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _dashboardService;

    public AdminDashboardController(IAdminDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Retrieves dashboard statistics for the authenticated administrator campus.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(AdminDashboardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
                return Unauthorized(new { message = "No se pudo identificar el usuario autenticado." });

            var dashboard = await _dashboardService.GetDashboardAsync(currentUserId.Value);
            return Ok(dashboard);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al obtener el dashboard administrativo", error = ex.Message });
        }
    }

    private int? GetCurrentUserId()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var userId) ? userId : null;
    }
}
