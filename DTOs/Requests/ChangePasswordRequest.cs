using System.ComponentModel.DataAnnotations;

namespace UNAPLANNER_API.DTOs.Requests;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "La contrasena actual es obligatoria")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contrasena es obligatoria")]
    [MinLength(8, ErrorMessage = "La nueva contrasena debe tener al menos 8 caracteres")]
    public string NewPassword { get; set; } = string.Empty;
}
