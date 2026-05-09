using System.ComponentModel.DataAnnotations;

namespace UNAPLANNER_API.DTOs.Requests;

public class CreateCampusContactRequest
{
    [Required]
    public int CampusId { get; set; }

    [Required(ErrorMessage = "El nombre del departamento es obligatorio")]
    [MaxLength(100)]
    public string DepartamentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El telefono es obligatorio")]
    [MaxLength(25)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Formato de correo invalido")]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
