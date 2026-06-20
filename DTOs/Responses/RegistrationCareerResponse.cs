namespace UNAPLANNER_API.DTOs.Responses;

public class RegistrationCareerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public List<RegistrationStudyPlanResponse> StudyPlans { get; set; } = new();
}

public class RegistrationStudyPlanResponse
{
    public int StudyPlanId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int ValidYear { get; set; }
}
