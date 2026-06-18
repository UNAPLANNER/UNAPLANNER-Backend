using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Responses;
public class StudentGPAResponse
{
    public int StudentId { get; set; }
    public decimal Gpa { get; set; }
    public string Message { get; set; } = string.Empty;
}