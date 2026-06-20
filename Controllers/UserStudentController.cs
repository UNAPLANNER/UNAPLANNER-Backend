using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserStudentController : ControllerBase
{
    private readonly IUserStudentService _userStudentService;

    public UserStudentController(IUserStudentService userStudentService)
    {
        _userStudentService = userStudentService;
    }

    private bool IsCurrentUser(int userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) && id == userId;
    }

    /// <summary>
    /// Deletes the current user and the associated student data
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(int id, [FromBody] DeleteUserRequest request)
    {
        if (!IsCurrentUser(id)) return Forbid();
        try
        {
            var deleted = await _userStudentService.DeleteUserStudent(id, request.CurrentPassword);

            if (!deleted)
                return Unauthorized(new { message = "Contraseña incorrecta o usuario no encontrado" });

            return Ok(new { message = $"Usuario ID {id} se ha eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al eliminar el usuario", error = ex.Message });
        }
    }
}