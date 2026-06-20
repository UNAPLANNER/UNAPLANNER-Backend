namespace UNAPLANNER_API.DTOs.Responses;

public class AdminDashboardCareerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int TotalCredits { get; set; }
    public int CourseCount { get; set; }
    public int? StudyPlanId { get; set; }
    public int? StudyPlanYear { get; set; }
    public bool IsStatus { get; set; }
    public DateTime CreatedDate { get; set; }
}
