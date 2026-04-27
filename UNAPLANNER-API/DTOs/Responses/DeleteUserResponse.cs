using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Requests;
public class DeleteUserResponse
{
    public string ExistingPassword { get; set; } = string.Empty;
    
}