namespace UNAPLANNER_API.DTOs.Responses;

public class StudentCourseProgressResponse
{
    public int CourseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public bool IsElective { get; set; }
    public string ElectiveType { get; set; } = "Obligatorio";
    public int Level { get; set; }
    public int Term { get; set; }
    public string Status { get; set; } = "Pendiente";
    public decimal? FinalGrade { get; set; }
    public int? Semester { get; set; }
    public int? Year { get; set; }
}
