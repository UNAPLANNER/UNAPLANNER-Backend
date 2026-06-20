using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;
using Microsoft.AspNetCore.Authorization;
using UNAPLANNER_API.Constants;


namespace UNAPLANNER_API.Controllers;

[Route("/api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("login")]

    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 400 Validación automática
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _authService.LoginAsync(request);

        // 401 credenciales inválidas
        if (result == null)
        {
            return Unauthorized();
        }

        return Ok(result);
    }

    /// <summary>
    /// Returns the current authenticated user's data decoded from the JWT.
    /// The frontend must call this after every login to get the authoritative session data.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var response = new AuthResponse
        {
            UserId = userId,
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty
        };

        if (int.TryParse(User.FindFirstValue("StudentId"), out var studentId))
        {
            response.StudentId = studentId;

            if (int.TryParse(User.FindFirstValue("CareerId"), out var careerId))
                response.CareerId = careerId;

            if (int.TryParse(User.FindFirstValue("StudyPlanId"), out var studyPlanId))
                response.StudyPlanId = studyPlanId;
        }

        return Ok(response);
    }

    /// <summary>
    /// Signals logout. The client must delete the token and clear all local session state.
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Sesión cerrada. Elimine el token del dispositivo." });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {

        if (!ModelState.IsValid)
        {
            var error = new
            {
                message = "Datos inválidos en la solicitud",
                errors = ModelState.Values
                                   .SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage)
            };

            return BadRequest(error);
        }

        try
        {
            request.RoleId = RoleContants.Student;
            var response = await _authService.RegisterUser(request);
            return CreatedAtAction(nameof(Register), new
            {
                message = "Estudiante creado exitosamente",
                data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al registrar el estudiante.", detail = ex.Message });
        }
    }

    
}