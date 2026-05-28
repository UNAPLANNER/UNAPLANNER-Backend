namespace UNAPLANNER_API.Models.Entities;

public class Admin
{
    public int AdminId { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public int CampusId { get; set; }

    public User User { get; set; } = null!;
    public Campus Campus { get; set; } = null!;
}
