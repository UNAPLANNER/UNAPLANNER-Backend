using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UNAPLANNER_API.DTOs.Requests;

public class UpdateEvaluationGradeRequest
{
    [JsonPropertyName("grade")]
    [Required]
    [Range(0, 100, ErrorMessage = "La nota debe estar entre 0 y 100.")]
    public decimal Grade { get; set; }
}
