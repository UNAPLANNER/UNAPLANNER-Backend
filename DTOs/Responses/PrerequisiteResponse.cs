namespace UNAPLANNER_API.DTOs.Responses;

public class PrerequisiteResponse
{
    public int CourseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string RequirementType { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
}
