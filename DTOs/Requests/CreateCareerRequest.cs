using System.ComponentModel.DataAnnotations;

namespace UNAPLANNER_API.DTOs.Requests;

public class CreateCareerRequest
{
    [Required(ErrorMessage = "El nombre de la carrera es obligatorio")]
    [StringLength(150, ErrorMessage = "El nombre de la carrera no puede exceder 150 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El codigo de la carrera es obligatorio")]
    [StringLength(20, ErrorMessage = "El codigo de la carrera no puede exceder 20 caracteres")]
    public string Code { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripcion no puede exceder 500 caracteres")]
    public string? Description { get; set; }

    [Range(1, 500, ErrorMessage = "El total de creditos debe estar entre 1 y 500")]
    public int TotalCredits { get; set; }

    public bool IsStatus { get; set; } = true;
}
