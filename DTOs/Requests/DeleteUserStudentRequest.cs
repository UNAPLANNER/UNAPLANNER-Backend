using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Requests;

public class DeleteUserRequest
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;
}
