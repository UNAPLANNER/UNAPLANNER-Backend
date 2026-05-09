namespace UNAPLANNER_API.DTOs.Requests;

public class CreateNoteRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    public int? CourseId { get; set; }
}
