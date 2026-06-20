namespace UNAPLANNER_API.DTOs.Responses;

public class StudyPlanCourseDetailResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public bool IsElective { get; set; }
    public string ElectiveType { get; set; } = "Obligatorio";
    public List<StudyPlanCourseRequirementResponse> Prerequisites { get; set; } = new();
}
