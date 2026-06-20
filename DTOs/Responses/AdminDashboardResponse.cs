namespace UNAPLANNER_API.DTOs.Responses;

public class AdminDashboardResponse
{
    public string AdminName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public int CampusId { get; set; }
    public string CampusName { get; set; } = string.Empty;
    public int ActiveCareers { get; set; }
    public int StudyPlans { get; set; }
    public int RegisteredCourses { get; set; }
    public int ActiveStudents { get; set; }
    public AdminDashboardCareerResponse? LatestCareer { get; set; }
    public List<AdminDashboardCareerResponse> Careers { get; set; } = new();
}
