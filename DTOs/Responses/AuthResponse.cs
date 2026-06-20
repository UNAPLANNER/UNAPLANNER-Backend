using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Responses;
public class AuthResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Token { get; set; }
    public int? StudentId { get; set; }
    public int? CareerId { get; set; }
    public int? StudyPlanId { get; set; }
}