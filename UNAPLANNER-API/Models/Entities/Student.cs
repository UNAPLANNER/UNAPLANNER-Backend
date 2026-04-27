namespace UNAPLANNER_API.Models.Entities;

public class Student
{
    public int StudentId { get; set; }

    public int UserId { get; set; }

    public int CareerId { get; set; }

    public int StudyPlanId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public int? EnterYear { get; set; }

    public bool IsStatus { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relaciones
    public User User { get; set; } = null!;
    public Career Career { get; set; } = null!;
    public StudyPlan StudyPlan { get; set; } = null!;
    public ICollection<StudentProgress> StudentProgress { get; set; } = new List<StudentProgress>();
}
