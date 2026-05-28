using System.ComponentModel.DataAnnotations;

namespace UNAPLANNER_API.DTOs.Requests;

public class UpdateProfileRequest
{
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El telefono es obligatorio")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "El departamento es obligatorio")]
    public string Department { get; set; } = string.Empty;
}
