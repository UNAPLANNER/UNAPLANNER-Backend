namespace UNAPLANNER_API.Models.Entities;

public class StudyPlan
{
    public int StudyPlanId { get; set; }

    public int CareerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public int ValidYear { get; set; }

    public bool IsStatus { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relaciones
    public Career Career { get; set; } = null!;
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new List<StudyPlanCourse>();
}