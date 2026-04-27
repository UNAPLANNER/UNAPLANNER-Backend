namespace UNAPLANNER_API.Models.Entities;

public class NotificationToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string FcmToken { get; set; } = string.Empty;

    public string? DeviceName { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public DateTime LastUpdated { get; set; } = DateTime.Now;

    // Relaciones
    public User User { get; set; } = null!;
}
