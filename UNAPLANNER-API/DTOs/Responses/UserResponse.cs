using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Responses;
public class UserResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public bool IsStatus { get; set; }
    public DateTime CreatedDate { get; set; }

}