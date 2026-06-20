using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize]
    /// Retrieves a student's profile using the UserId
    [HttpGet("{userId}/profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProfile(int userId)
    {
        try
        {
            var result = await _userStudentService.GetStudentProfile(userId);

            if (result == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al obtener perfil", error = ex.Message });
        }
    }

    /**Updates the profile information of a student by userId.
    The ID of the user whose profile will be updated
    Returns 200 OK if the update is successful,
    400 BadRequest if the data is invalid or the student is not found,
    500 InternalServerError if an unexpected error occurs**/

    [Authorize]
    [HttpPut("{userId}/profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UpdateStudentProfileRequest dto)
    {
        try
        {
            var result = await _userStudentService.UpdateProfileStudent(userId, dto);

            if (result == null)
                return BadRequest(new { message = "Datos inválidos o estudiante no encontrado" });

            return Ok(new
            {
                message = "Perfil actualizado exitosamente",
                data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error updating profile", error = ex.Message });
        }
    }
}