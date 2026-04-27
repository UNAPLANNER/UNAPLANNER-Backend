using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;
using Microsoft.AspNetCore.Authorization;


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
            var response = await _authService.RegisterUser(request);
            return CreatedAtAction(nameof(Register), new
            {
                message = "Usuario creado exitosamente",
                data = response
            });

        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}