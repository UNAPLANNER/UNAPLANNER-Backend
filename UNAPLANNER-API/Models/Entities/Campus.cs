namespace UNAPLANNER_API.Models.Entities;

public class Campus
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool IsStatus { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relaciones
    public ICollection<Career> Careers { get; set; } = new List<Career>();
    public ICollection<CampusContact> CampusContacts { get; set; } = new List<CampusContact>();
}
