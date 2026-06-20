using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Responses;
public class AuthResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}