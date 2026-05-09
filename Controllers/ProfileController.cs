using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { message = "Token invalido" });

        var profile = await _profileService.GetProfileAsync(userId.Value);
        if (profile == null) return NotFound(new { message = "Perfil no encontrado" });

        return Ok(profile);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(int id)
    {
        var profile = await _profileService.GetProfileAsync(id);
        if (profile == null) return NotFound(new { message = "Perfil no encontrado" });

        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { message = "Token invalido" });

        var updatedProfile = await _profileService.UpdateProfileAsync(userId.Value, request);
        if (updatedProfile == null) return BadRequest(new { message = "No se pudo actualizar el perfil" });

        return Ok(updatedProfile);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileRequest request)
    {
        var updatedProfile = await _profileService.UpdateProfileAsync(id, request);
        if (updatedProfile == null) return BadRequest(new { message = "No se pudo actualizar el perfil" });

        return Ok(updatedProfile);
    }

    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangeMyPassword([FromBody] ChangePasswordRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { message = "Token invalido" });

        var result = await _profileService.ChangePasswordAsync(userId.Value, request);
        if (!result) return BadRequest(new { message = "La contrasena actual es incorrecta o no se pudo actualizar" });

        return Ok(new { message = "Contrasena actualizada exitosamente" });
    }

    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        var result = await _profileService.ChangePasswordAsync(id, request);
        if (!result) return BadRequest(new { message = "La contrasena actual es incorrecta o no se pudo actualizar" });

        return Ok(new { message = "Contrasena actualizada exitosamente" });
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
