namespace UNAPLANNER_API.DTOs.Responses;

public class CampusContactResponse
{
    public int Id { get; set; }

    public int CampusId { get; set; }

    public string DepartamentName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Description { get; set; }

    public bool IsStatus { get; set; }

    public DateTime CreatedDate { get; set; }
}
