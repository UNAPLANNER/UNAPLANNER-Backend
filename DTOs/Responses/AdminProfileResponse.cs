namespace UNAPLANNER_API.DTOs.Responses;

public class AdminProfileResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public int CampusId { get; set; }
}
