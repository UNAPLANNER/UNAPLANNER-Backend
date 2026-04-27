namespace UNAPLANNER_API.Models.Entities;

public class CampusContact
{
    public int Id { get; set; }

    public int CampusId { get; set; }

    public string DepartamentName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Description { get; set; }

    public bool IsStatus { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relaciones
    public Campus Campus { get; set; } = null!;
}
