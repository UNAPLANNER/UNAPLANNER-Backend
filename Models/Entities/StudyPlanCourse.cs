namespace UNAPLANNER_API.Models.Entities;

public class StudyPlanCourse
{
    public int Id { get; set; }

    public int StudyPlanId { get; set; }

    public int CourseId { get; set; }

    public int Levels { get; set; }

    public int Term { get; set; }

    public bool IsElective { get; set; } = false;

    public string ElectiveType { get; set; } = "Obligatorio"; // Obligatorio | OptativoDisciplinario | OptativoLibre

    public bool IsStatus { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relaciones
    public StudyPlan StudyPlan { get; set; } = null!;
    public Course Course { get; set; } = null!;
}