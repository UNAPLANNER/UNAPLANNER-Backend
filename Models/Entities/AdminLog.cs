namespace UNAPLANNER_API.Models.Entities;

public class AdminLog
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string Entity { get; set; } = string.Empty;

    public int? EntityId { get; set; }

    public string? Detail { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relaciones
    public User User { get; set; } = null!;
}