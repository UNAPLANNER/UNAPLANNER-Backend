namespace UNAPLANNER_API.DTOs.Responses;

public class NoteResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    public int? CourseId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
