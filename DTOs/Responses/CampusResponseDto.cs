using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Responses;

public class CampusResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public bool IsStatus { get; set; }
}