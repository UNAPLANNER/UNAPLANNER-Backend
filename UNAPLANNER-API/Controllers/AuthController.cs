using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;

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
}