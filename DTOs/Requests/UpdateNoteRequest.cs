using System.ComponentModel.DataAnnotations;

namespace UNAPLANNER_API.DTOs.Requests;

public class UpdateNoteRequest
{
    [Required(ErrorMessage = "El titulo es requerido")]
    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    public int? CourseId { get; set; }
}
