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
}