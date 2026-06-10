namespace UNAPLANNER_API.DTOs.Responses;

public class StudentCurriculumResponse
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int CareerId { get; set; }
    public string CareerName { get; set; } = string.Empty;
    public string CareerCode { get; set; } = string.Empty;
    public int StudyPlanId { get; set; }
    public string StudyPlanName { get; set; } = string.Empty;
    public int StudyPlanYear { get; set; }
    public List<LevelCurriculumResponse> Levels { get; set; } = new();
}
