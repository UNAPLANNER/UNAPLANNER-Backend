using System.ComponentModel.DataAnnotations;

namespace UNAPLANNER_API.DTOs.Requests;

public class UpdateCourseStatusRequest
{
    [Required]
    [RegularExpression("^(Pendiente|EnCurso|Aprobado|Reprobado)$",
        ErrorMessage = "El estado debe ser: Pendiente, EnCurso, Aprobado o Reprobado")]
    public string Status { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "La nota final debe estar entre 0 y 100")]
    public decimal? FinalGrade { get; set; }

    [Range(1, 2, ErrorMessage = "El semestre académico debe ser 1 o 2")]
    public int? Semester { get; set; }

    public int? Year { get; set; }
}
