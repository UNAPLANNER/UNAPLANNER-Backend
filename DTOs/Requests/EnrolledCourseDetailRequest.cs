using System.ComponentModel.DataAnnotations;

namespace UNAPLANNER_API.DTOs.Requests;

public class EnrolledCourseDetailRequest
{
    [MaxLength(200)]
    public string? ProfessorName { get; set; }

    [MaxLength(100)]
    public string? Classroom { get; set; }

    [MaxLength(300)]
    public string? Schedule { get; set; }

    [MaxLength(500)]
    public string? SyllabusUrl { get; set; }
}
