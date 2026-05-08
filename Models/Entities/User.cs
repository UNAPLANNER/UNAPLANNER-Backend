namespace UNAPLANNER_API.Models.Entities;
public class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty; 

    public bool IsStatus { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relación (muchos a 1)
    public Role Role { get; set; } = null!;
    public Student Student { get; set; } = null!;
    public Admin Admin { get; set; } = null!;

    
    public ICollection<AdminLog> AdminLogs { get; set; } = new List<AdminLog>();
    public ICollection<File> Files { get; set; } = new List<File>();
    public ICollection<Calendar> Calendars { get; set; } = new List<Calendar>();
    public ICollection<Note> Notes { get; set; } = new List<Note>();
    public ICollection<NotificationToken> NotificationTokens { get; set; } = new List<NotificationToken>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}