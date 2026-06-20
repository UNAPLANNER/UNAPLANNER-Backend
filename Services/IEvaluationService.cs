using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;
//
public interface IEvaluationService
{
    Task<(bool Success, CourseEvaluationsResponse? Data, string? ErrorMessage)> GetCourseEvaluationsAsync(int courseId, int studentId);
    Task<(bool Success, EvaluationResponse? Evaluation, string? ErrorMessage)> CreateEvaluationAsync(int courseId, int studentId, CreateEvaluationRequest request);
    Task<(bool Success, EvaluationResponse? Evaluation, string? ErrorMessage)> UpdateEvaluationAsync(int evaluationId, int studentId, UpdateEvaluationRequest request);
    Task<(bool Success, string? ErrorMessage)> DeleteEvaluationAsync(int evaluationId, int studentId);
}
