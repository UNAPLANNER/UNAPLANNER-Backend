using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UNAPLANNER_API.DTOs.Requests;

public class UpdateStudyPlanCourseRequest
{
    [Required(ErrorMessage = "El codigo del curso es obligatorio.")]
    [StringLength(450, ErrorMessage = "El codigo del curso no puede exceder 450 caracteres.")]
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del curso es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre del curso no puede exceder 150 caracteres.")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "Los creditos deben estar entre 1 y 20.")]
    [JsonPropertyName("credits")]
    public int Credits { get; set; }

    [Range(0, 20, ErrorMessage = "Las horas teoria deben estar entre 0 y 20.")]
    [JsonPropertyName("theoryHours")]
    public int TheoryHours { get; set; }

    [Range(0, 20, ErrorMessage = "Las horas practica deben estar entre 0 y 20.")]
    [JsonPropertyName("practiceHours")]
    public int PracticeHours { get; set; }

    [Range(0, 20, ErrorMessage = "Las horas laboratorio deben estar entre 0 y 20.")]
    [JsonPropertyName("labHours")]
    public int LabHours { get; set; }

    [Range(1, 4, ErrorMessage = "El nivel debe estar entre 1 y 4.")]
    [JsonPropertyName("level")]
    public int Level { get; set; }

    [Range(1, 2, ErrorMessage = "El ciclo debe ser 1 o 2.")]
    [JsonPropertyName("term")]
    public int Term { get; set; }

    [JsonPropertyName("isElective")]
    public bool IsElective { get; set; }

    [Required(ErrorMessage = "El tipo de curso es obligatorio.")]
    [JsonPropertyName("electiveType")]
    public string ElectiveType { get; set; } = "Obligatorio";

    [JsonPropertyName("isStatus")]
    public bool IsStatus { get; set; } = true;

    [JsonPropertyName("prerequisiteCourseIds")]
    public List<int> PrerequisiteCourseIds { get; set; } = new();
}
