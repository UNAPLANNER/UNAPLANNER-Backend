namespace UNAPLANNER_API.DTOs.Responses;

public class CourseDetailResponse
{
    public int CourseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int TheoryHours { get; set; }
    public int PracticeHours { get; set; }
    public int LabHours { get; set; }
    public int Level { get; set; }
    public int Term { get; set; }
    public bool IsElective { get; set; }
    public string ElectiveType { get; set; } = "Obligatorio";
    public string Status { get; set; } = "Pendiente";
    public decimal? FinalGrade { get; set; }
    public int? AcademicTerm { get; set; }
    public int? TermYear { get; set; }
    public List<PrerequisiteResponse> Prerequisites { get; set; } = new();

    // Only populated when Status is EnCurso or Aprobado
    public string? ProfessorName { get; set; }
    public string? Classroom { get; set; }
    public string? Schedule { get; set; }
    public string? SyllabusUrl { get; set; }
}
