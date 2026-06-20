using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El grado de la carrera es obligatorio")]
    [StringLength(50, ErrorMessage = "El grado no puede exceder 50 caracteres")]
    [JsonPropertyName("degreeLevel")]
    public string DegreeLevel { get; set; } = string.Empty;

    [Range(1900, 9999, ErrorMessage = "El año del plan no es valido")]
    [JsonPropertyName("planYear")]
    public int PlanYear { get; set; }

    [Required(ErrorMessage = "La escuela es obligatoria")]
    [StringLength(150, ErrorMessage = "La escuela no puede exceder 150 caracteres")]
    [JsonPropertyName("school")]
    public string School { get; set; } = string.Empty;

    [Range(1, 500, ErrorMessage = "Los creditos de bachillerato deben estar entre 1 y 500")]
    [JsonPropertyName("bachelorCredits")]
    public int? BachelorCredits { get; set; }

    [Range(1, 500, ErrorMessage = "Los creditos de diplomado deben estar entre 1 y 500")]
    [JsonPropertyName("diplomaCredits")]
    public int? DiplomaCredits { get; set; }

    [Range(1, 500, ErrorMessage = "Los creditos del grado deben estar entre 1 y 500")]
    [JsonPropertyName("degreeCredits")]
    public int? DegreeCredits { get; set; }

    [Required(ErrorMessage = "La resolucion oficial es obligatoria")]
    [StringLength(20, ErrorMessage = "La resolucion oficial no puede exceder 20 caracteres")]
    [JsonPropertyName("officialResolution")]
    public string OfficialResolution { get; set; } = string.Empty;

    [JsonPropertyName("isStatus")]
    public bool IsStatus { get; set; } = true;
}
