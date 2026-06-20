using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;
// Service for managing evaluations in the UNAPLANNER API
public class EvaluationService : IEvaluationService
{
    private static readonly HashSet<string> ValidEvaluationTypes =
        new(StringComparer.OrdinalIgnoreCase) { "Examen", "Tarea", "Proyecto", "Quiz", "Exposicion", "Otro" };

    private readonly IEvaluationRepository _evaluationRepository;
//Constructor for the EvaluationService, injecting the evaluation repository
    public EvaluationService(IEvaluationRepository evaluationRepository)
    {
        _evaluationRepository = evaluationRepository;
    }
//Method to get evaluations for a specific course and student, including total percentage and calculated final grade
    public async Task<(bool Success, CourseEvaluationsResponse? Data, string? ErrorMessage)> GetCourseEvaluationsAsync(int courseId, int studentId)
    {
        try
        {
            var progress = await _evaluationRepository.GetStudentProgressAsync(studentId, courseId);
            if (progress == null)
                return (false, null, $"No se encontró progreso para el estudiante en el curso {courseId}.");

            var evaluations = await _evaluationRepository.GetEvaluationsByStudentProgressAsync(progress.StudentProgressId);

            var evaluationResponses = evaluations.Select(MapToResponse).ToList();

            var totalPercentage = evaluations.Sum(e => e.Percentage);
            decimal? calculatedFinalGrade = evaluations.Any(e => e.Score.HasValue)
                ? evaluations.Where(e => e.Score.HasValue).Sum(e => e.Score!.Value * e.Percentage / 100)
                : null;

            var response = new CourseEvaluationsResponse
            {
                Evaluations = evaluationResponses,
                TotalPercentage = totalPercentage,
                CalculatedFinalGrade = calculatedFinalGrade
            };

            return (true, response, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al obtener las evaluaciones: {ex.Message}");
        }
    }
//Method to create a new evaluation for a student's course, validating input and ensuring percentage limits are respected
    public async Task<(bool Success, EvaluationResponse? Evaluation, string? ErrorMessage)> CreateEvaluationAsync(int courseId, int studentId, CreateEvaluationRequest request)
    {
        try
        {
            var typeError = ValidateEvaluationType(request.EvaluationType);
            if (typeError != null) return (false, null, typeError);

            var progress = await _evaluationRepository.GetStudentProgressAsync(studentId, courseId);
            if (progress == null)
                return (false, null, $"No se encontró progreso para el estudiante en el curso {courseId}.");

            var existing = await _evaluationRepository.GetEvaluationsByStudentProgressAsync(progress.StudentProgressId);
            var usedPercentage = existing.Sum(e => e.Percentage);
            if (usedPercentage + request.Percentage > 100)
                return (false, null, $"La suma de porcentajes superaría el 100%. Porcentaje disponible: {100 - usedPercentage:0.##}%.");

            var evaluation = new Evaluation
            {
                StudentProgressId = progress.StudentProgressId,
                Name = request.Name,
                Percentage = request.Percentage,
                Score = null,
                Date = request.Date,
                EvaluationType = request.EvaluationType,
                CreatedDate = DateTime.Now
            };

            var created = await _evaluationRepository.CreateEvaluationAsync(evaluation);
            if (created == null)
                return (false, null, "Error al guardar la evaluación en la base de datos.");

            return (true, MapToResponse(created), null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al crear la evaluación: {ex.Message}");
        }
    }
//Method to update an existing evaluation, validating input and ensuring percentage limits are respected, and checking permissions
    public async Task<(bool Success, EvaluationResponse? Evaluation, string? ErrorMessage)> UpdateEvaluationAsync(int evaluationId, int studentId, UpdateEvaluationRequest request)
    {
        try
        {
            var typeError = ValidateEvaluationType(request.EvaluationType);
            if (typeError != null) return (false, null, typeError);

            var evaluation = await _evaluationRepository.GetEvaluationByIdAsync(evaluationId);
            if (evaluation == null)
                return (false, null, $"Evaluación con ID {evaluationId} no encontrada.");

            if (evaluation.StudentProgress.StudentId != studentId)
                return (false, null, "No tienes permisos para modificar esta evaluación.");

            var others = await _evaluationRepository.GetEvaluationsByStudentProgressAsync(evaluation.StudentProgressId);
            var usedPercentage = others.Where(e => e.Id != evaluationId).Sum(e => e.Percentage);
            if (usedPercentage + request.Percentage > 100)
                return (false, null, $"La suma de porcentajes superaría el 100%. Porcentaje disponible: {100 - usedPercentage:0.##}%.");

            evaluation.Name = request.Name;
            evaluation.EvaluationType = request.EvaluationType;
            evaluation.Percentage = request.Percentage;
            evaluation.Score = request.Score;
            evaluation.Date = request.Date;

            var updated = await _evaluationRepository.UpdateEvaluationAsync(evaluation);
            if (updated == null)
                return (false, null, "Error al actualizar la evaluación en la base de datos.");

            return (true, MapToResponse(updated), null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al actualizar la evaluación: {ex.Message}");
        }
    }
//Method to delete an evaluation, checking permissions and handling errors
    public async Task<(bool Success, string? ErrorMessage)> DeleteEvaluationAsync(int evaluationId, int studentId)
    {
        try
        {
            var evaluation = await _evaluationRepository.GetEvaluationByIdAsync(evaluationId);
            if (evaluation == null)
                return (false, $"Evaluación con ID {evaluationId} no encontrada.");

            if (evaluation.StudentProgress.StudentId != studentId)
                return (false, "No tienes permisos para eliminar esta evaluación.");

            var deleted = await _evaluationRepository.DeleteEvaluationAsync(evaluationId);
            if (!deleted)
                return (false, "Error al eliminar la evaluación de la base de datos.");

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al eliminar la evaluación: {ex.Message}");
        }
    }
// Helper method to map an Evaluation entity to an EvaluationResponse DTO
    private static EvaluationResponse MapToResponse(Evaluation e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        EvaluationType = e.EvaluationType,
        Percentage = e.Percentage,
        Score = e.Score,
        Date = e.Date
    };
// Helper method to validate the evaluation type against a predefined set of valid types
    private static string? ValidateEvaluationType(string evaluationType)
    {
        if (!ValidEvaluationTypes.Contains(evaluationType))
            return $"Tipo de evaluación inválido. Tipos permitidos: {string.Join(", ", ValidEvaluationTypes)}.";
        return null;
    }
}
