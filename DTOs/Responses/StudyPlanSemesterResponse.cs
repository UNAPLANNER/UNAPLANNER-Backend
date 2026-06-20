namespace UNAPLANNER_API.DTOs.Responses;

public class StudyPlanSemesterResponse
{
    public int Semester { get; set; }
    public List<StudyPlanCourseDetailResponse> Courses { get; set; } = new();
}
