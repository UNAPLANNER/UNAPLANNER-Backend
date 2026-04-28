using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Requests;
public class DeleteUserResponse
{
    public int UserId { get; set; }
    public string ExistingPassword { get; set; } = string.Empty;
    public string Message { get; set; }  = string.Empty;
    
}