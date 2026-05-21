namespace UNAPLANNER_API.DTOs.Responses;

public class CareerResponse
{
    public int Id { get; set; }

    public int CampusId { get; set; }

    public string CampusName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int TotalCredits { get; set; }

    public int? CurrentStudyPlanId { get; set; }

    public string? CurrentStudyPlanName { get; set; }

    public int? CurrentStudyPlanYear { get; set; }

    public int CourseCount { get; set; }

    public int LevelCount { get; set; }

    public bool IsStatus { get; set; }

    public DateTime CreatedDate { get; set; }
}
