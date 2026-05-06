namespace UNAPLANNER_API.Models.Entities;

public class StudentCourseDetail
{
    public int Id { get; set; }

    public int StudentProgressId { get; set; }

    public string? ProfessorName { get; set; }

    public string? Classroom { get; set; }

    public string? Schedule { get; set; }

    public string? SyllabusUrl { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relaciones
    public StudentProgress StudentProgress { get; set; } = null!;
}