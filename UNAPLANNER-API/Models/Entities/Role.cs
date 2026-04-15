namespace UNAPLANNER_API.Models.Entities;

public class Role
{
    public int RoleId { get; set; }

    public string TypeRole { get; set; } = string.Empty;

    // Relación con User (1 a muchos)
    public ICollection<User> Users { get; set; } = new List<User>();
}