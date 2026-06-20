namespace UNAPLANNER_API.DTOs.Responses;

public class StudyPlanDetailResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int CareerId { get; set; }
    public string CareerName { get; set; } = string.Empty;
    public string CareerCode { get; set; } = string.Empty;
    public int EffectiveYear { get; set; }
    public int TotalCredits { get; set; }
    public int CourseCount { get; set; }
    public int LevelCount { get; set; }
    public int CycleCount { get; set; }
    public List<StudyPlanLevelResponse> Levels { get; set; } = new();
}
