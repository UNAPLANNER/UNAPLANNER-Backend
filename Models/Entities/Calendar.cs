namespace UNAPLANNER_API.Models.Entities;

public class Calendar
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? CourseId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime ActivityDate { get; set; }

    public string ActivityType { get; set; } = string.Empty; // 'Examen', 'Tarea', 'Proyecto', etc.

    public bool HasReminder { get; set; } = false;

    public DateTime? ReminderDate { get; set; }

    public bool IsCompleted { get; set; } = false;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relaciones
    public User User { get; set; } = null!;
    public Course? Course { get; set; }
}