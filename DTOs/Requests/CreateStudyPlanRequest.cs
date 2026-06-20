using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UNAPLANNER_API.DTOs.Requests;

public class CreateStudyPlanRequest
{
    [Required(ErrorMessage = "La carrera es obligatoria.")]
    [JsonPropertyName("careerId")]
    public int CareerId { get; set; }

    [Required(ErrorMessage = "El nombre del plan es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre del plan no puede exceder 150 caracteres.")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El codigo del plan es obligatorio.")]
    [StringLength(450, ErrorMessage = "El codigo del plan no puede exceder 450 caracteres.")]
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [Range(1900, 9999, ErrorMessage = "El año de vigencia no es valido.")]
    [JsonPropertyName("validYear")]
    public int ValidYear { get; set; }

    [JsonPropertyName("isStatus")]
    public bool IsStatus { get; set; } = true;
}
