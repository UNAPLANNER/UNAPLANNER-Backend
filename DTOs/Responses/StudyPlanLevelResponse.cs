namespace UNAPLANNER_API.DTOs.Responses;

public class StudyPlanLevelResponse
{
    public int Level { get; set; }
    public int Credits { get; set; }
    public List<StudyPlanSemesterResponse> Semesters { get; set; } = new();
}
