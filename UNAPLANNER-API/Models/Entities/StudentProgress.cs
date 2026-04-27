namespace UNAPLANNER_API.Models.Entities;

public class StudentProgress
{
    public int StudentProgressId { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public string Status { get; set; } = "Pendiente"; // 'Pendiente', 'EnCurso', 'Aprobado', 'Reprobado'

    public decimal? FinalGrade { get; set; }

    public int? TermYear { get; set; }

    public int? AcademicTerm { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.Now;

    // Relaciones
    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public StudentCourseDetail? StudentCourseDetail { get; set; }
    public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
}
