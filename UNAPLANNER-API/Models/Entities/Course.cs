namespace UNAPLANNER_API.Models.Entities;

public class Course
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Credits { get; set; } = 3;

    public int TheoryHours { get; set; } = 0;

    public int PracticeHours { get; set; } = 0;

    public int LabHours { get; set; } = 0;

    public bool IsStatus { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relaciones
    public ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new List<StudyPlanCourse>();
    public ICollection<Requirement> CourseRequirements { get; set; } = new List<Requirement>();
    public ICollection<Requirement> RequiredByRequirements { get; set; } = new List<Requirement>();
    public ICollection<StudentProgress> StudentProgress { get; set; } = new List<StudentProgress>();
    public ICollection<Calendar> Calendars { get; set; } = new List<Calendar>();
    public ICollection<File> Files { get; set; } = new List<File>();
    public ICollection<Note> Notes { get; set; } = new List<Note>();
}