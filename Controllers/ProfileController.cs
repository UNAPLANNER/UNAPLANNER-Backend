using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(int id)
    {
        var profile = await _profileService.GetProfileAsync(id);
        if (profile == null) return NotFound(new { message = "Perfil no encontrado" });

        return Ok(profile);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileRequest request)
    {
        var updatedProfile = await _profileService.UpdateProfileAsync(id, request);
        if (updatedProfile == null) return BadRequest(new { message = "No se pudo actualizar el perfil" });

        return Ok(updatedProfile);
    }

    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        var result = await _profileService.ChangePasswordAsync(id, request);
        if (!result) return BadRequest(new { message = "La contraseña actual es incorrecta o no se pudo actualizar" });

        return Ok(new { message = "Contraseña actualizada exitosamente" });
    }
}
