using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;
// Interface for handling evaluations in the UNAPLANNER API
public interface IEvaluationRepository
{
    Task<StudentProgress?> GetStudentProgressAsync(int studentId, int courseId);
    Task<Evaluation?> GetEvaluationByIdAsync(int evaluationId);
    Task<List<Evaluation>> GetEvaluationsByStudentProgressAsync(int studentProgressId);
    Task<Evaluation?> CreateEvaluationAsync(Evaluation evaluation);
    Task<Evaluation?> UpdateEvaluationAsync(Evaluation evaluation);
    Task<bool> DeleteEvaluationAsync(int evaluationId);
}
