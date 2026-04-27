namespace UNAPLANNER_API.Models.Entities;

public class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty; // 'CourseApproved', 'ExamReminder', etc.

    public int? RelatedId { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public DateTime? SentDate { get; set; }

    // Relaciones
    public User User { get; set; } = null!;
}
