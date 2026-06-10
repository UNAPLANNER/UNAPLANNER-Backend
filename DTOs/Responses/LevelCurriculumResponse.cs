namespace UNAPLANNER_API.DTOs.Responses;

public class LevelCurriculumResponse
{
    public int Level { get; set; }
    public List<SemesterCoursesResponse> Semesters { get; set; } = new();
}
