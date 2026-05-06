namespace UNAPLANNER_API.Models.Entities;

public class Career
{
    public int Id { get; set; }

    public int CampusId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int TotalCredits { get; set; } = 0;

    public bool IsStatus { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relaciones
    public Campus Campus { get; set; } = null!;
    public ICollection<StudyPlan> StudyPlans { get; set; } = new List<StudyPlan>();
    public ICollection<Student> Students { get; set; } = new List<Student>();
}