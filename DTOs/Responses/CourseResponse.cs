namespace UNAPLANNER_API.DTOs.Responses;

public class CourseResponse
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Credits { get; set; }

    public int TheoryHours { get; set; }

    public int PracticeHours { get; set; }

    public int LabHours { get; set; }
}
