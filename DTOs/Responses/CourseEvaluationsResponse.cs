namespace UNAPLANNER_API.DTOs.Responses;

public class CourseEvaluationsResponse
{
    public List<EvaluationResponse> Evaluations { get; set; } = new();
    public decimal TotalPercentage { get; set; }
    public decimal? CalculatedFinalGrade { get; set; }
}
