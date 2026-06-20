using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Responses;

public class UpdateStudentResponseDto
{
    public int StudentId { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int CareerId { get; set; }
    public string CareerName { get; set; } = string.Empty;
    public int? EnterYear { get; set; }
    public string Email { get; set; } = string.Empty; 
}