using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UNAPLANNER_API.DTOs.Requests;

public class UpdateEvaluationRequest
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    [Required(ErrorMessage = "El tipo de evaluación es requerido.")]
    public string EvaluationType { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 100, ErrorMessage = "El porcentaje debe estar entre 0.01 y 100.")]
    public decimal Percentage { get; set; }

    [JsonPropertyName("grade")]
    [Range(0, 100, ErrorMessage = "La nota debe estar entre 0 y 100.")]
    public decimal? Score { get; set; }

    public DateTime? Date { get; set; }
}
