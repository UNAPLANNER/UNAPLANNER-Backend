using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Requests;

public class CreateUserRequest
{
    public int RoleId { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
