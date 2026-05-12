using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("/api/profile")]
public class ProfileController : ControllerBase
{
    private static readonly HashSet<string> AllowedUpdateFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "fullName",
        "phone",
        "department"
    };

    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProfile(int id)
    {
        if (!IsCurrentUser(id)) return Forbid();

        var profile = await _profileService.GetAdminProfileAsync(id);
        return profile == null ? NotFound(new { message = "Administrador no encontrado" }) : Ok(profile);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] JsonElement body)
    {
        if (!IsCurrentUser(id)) return Forbid();

        var bodyValidation = ValidateUpdateBody(body);
        if (bodyValidation != null) return bodyValidation;

        var request = body.Deserialize<UpdateProfileRequest>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (request == null) return BadRequest(new { message = "El cuerpo de la solicitud es obligatorio" });

        ValidateUpdateRequest(request);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var updatedProfile = await _profileService.UpdateAdminProfileAsync(id, request);
        return updatedProfile == null ? NotFound(new { message = "Administrador no encontrado" }) : Ok(updatedProfile);
    }

    [HttpPost("{id:int}/change-password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        if (!IsCurrentUser(id)) return Forbid();
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var result = await _profileService.ChangePasswordAsync(id, request);

        return result switch
        {
            PasswordChangeResult.Success => NoContent(),
            PasswordChangeResult.InvalidCurrentPassword => Unauthorized(new { message = "La contrasena actual es incorrecta" }),
            PasswordChangeResult.AdminNotFound => NotFound(new { message = "Administrador no encontrado" }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al cambiar la contrasena" })
        };
    }

    private IActionResult? ValidateUpdateBody(JsonElement body)
    {
        if (body.ValueKind != JsonValueKind.Object)
        {
            return BadRequest(new { message = "El cuerpo debe ser un objeto JSON" });
        }

        var fields = body.EnumerateObject().Select(property => property.Name).ToList();
        var invalidFields = fields.Where(field => !AllowedUpdateFields.Contains(field)).ToList();

        // Email and role are intentionally blocked to preserve permissions and identity consistency.
        if (invalidFields.Count > 0)
        {
            return BadRequest(new
            {
                message = "El perfil solo permite actualizar fullName, phone y department",
                invalidFields
            });
        }

        var missingFields = AllowedUpdateFields
            .Where(required => fields.All(field => !string.Equals(field, required, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        return missingFields.Count > 0
            ? BadRequest(new { message = "Faltan campos obligatorios", missingFields })
            : null;
    }

    private void ValidateUpdateRequest(UpdateProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            ModelState.AddModelError(nameof(request.FullName), "El nombre completo es obligatorio");
        }

        if (string.IsNullOrWhiteSpace(request.Department))
        {
            ModelState.AddModelError(nameof(request.Department), "El departamento es obligatorio");
        }

        if (string.IsNullOrWhiteSpace(request.Phone) || !ProfileService.IsValidInstitutionalPhone(request.Phone.Trim()))
        {
            ModelState.AddModelError(nameof(request.Phone), "El telefono debe tener el formato institucional 2277-XXXX");
        }
    }

    private bool IsCurrentUser(int id)
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var currentUserId) && currentUserId == id;
    }
}
