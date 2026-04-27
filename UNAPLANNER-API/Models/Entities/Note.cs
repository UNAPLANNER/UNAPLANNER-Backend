namespace UNAPLANNER_API.Models.Entities;

public class Note
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? CourseId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime LastUpdated { get; set; } = DateTime.Now;

    // Relaciones
    public User User { get; set; } = null!;
    public Course? Course { get; set; }
}
