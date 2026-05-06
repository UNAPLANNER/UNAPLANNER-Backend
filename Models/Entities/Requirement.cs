namespace UNAPLANNER_API.Models.Entities;

public class Requirement
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int RequiredCourseId { get; set; }

    public string RequirementType { get; set; } = string.Empty; // 'Prerequisite' or 'Corequisite'

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relaciones
    public Course Course { get; set; } = null!;
    public Course RequiredCourse { get; set; } = null!;
}