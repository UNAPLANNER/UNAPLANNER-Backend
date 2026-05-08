using System.ComponentModel.DataAnnotations;

namespace UNAPLANNER_API.DTOs.Requests;

public class CreateCampusContactRequest
{
    [Required(ErrorMessage = "El nombre del departamento es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre del departamento no puede exceder 100 caracteres")]
    public string DepartamentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [RegularExpression(@"^[\d\s\-\(\)\.]{7,25}(\s?[eE]xt\.?\s*\d{1,6})?$",
        ErrorMessage = "Formato de teléfono inválido. Solo se permiten números, guiones, espacios y extensión opcional (ej: 2766-6001 ext 123)")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campus es obligatorio")]
    public int CampusId { get; set; }

    [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
    public string? Email { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Description { get; set; }
}
