namespace UNAPLANNER_API.DTOs.Responses;

public class SemesterCoursesResponse
{
    public int Semester { get; set; }
    public List<CurriculumCourseResponse> Courses { get; set; } = new();
}
