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
    private readonly ICalendarRepository _calendarRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;

    public EvaluationService(
        IEvaluationRepository evaluationRepository,
        ICalendarRepository calendarRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService)
    {
        _evaluationRepository = evaluationRepository;
        _calendarRepository = calendarRepository;
        _studentRepository = studentRepository;
        _notificationService = notificationService;
    }

    public async Task<(bool Success, CourseEvaluationsResponse? Data, string? ErrorMessage)> GetCourseEvaluationsAsync(int courseId, int studentId)
    {
        try
        {
            var progress = await _evaluationRepository.GetStudentProgressAsync(studentId, courseId);
            if (progress == null)
                return (false, null, $"No se encontró progreso para el estudiante en el curso {courseId}.");

            var evaluations = await _evaluationRepository.GetEvaluationsByStudentProgressAsync(progress.StudentProgressId);

            var evalIds = evaluations.Select(e => e.Id);
            var evalIdsWithReminder = await _calendarRepository.GetEvaluationIdsWithReminderAsync(evalIds);

            var evaluationResponses = evaluations.Select(e => MapToResponse(e, evalIdsWithReminder.Contains(e.Id))).ToList();

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

            if (request.HasReminder && !request.Date.HasValue)
                return (false, null, "Se requiere una fecha para agregar un recordatorio al calendario.");

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

            if (request.HasReminder && created.Date.HasValue)
                await SyncCalendarCreateAsync(studentId, courseId, created);

            return (true, MapToResponse(created, request.HasReminder), null);
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

            if (request.HasReminder && !request.Date.HasValue)
                return (false, null, "Se requiere una fecha para agregar un recordatorio al calendario.");

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

            await SyncCalendarUpdateAsync(studentId, updated, request.HasReminder);

            return (true, MapToResponse(updated, request.HasReminder), null);
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

            await _calendarRepository.DeleteByEvaluationIdAsync(evaluationId);

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

    private static readonly HashSet<string> ValidCalendarTypes =
        new(StringComparer.OrdinalIgnoreCase) { "Examen", "Tarea", "Proyecto", "Exposicion", "Evento", "Otro" };

    private async Task SyncCalendarCreateAsync(int studentId, int courseId, Evaluation evaluation)
    {
        var student = await _studentRepository.GetStudentByIdAsync(studentId);
        if (student == null) return;

        var calendarType = ValidCalendarTypes.Contains(evaluation.EvaluationType) ? evaluation.EvaluationType : "Otro";
        var reminderDate = evaluation.Date!.Value.AddDays(-1);

        var created = await _calendarRepository.CreateAsync(new Calendar
        {
            UserId = student.UserId,
            CourseId = courseId,
            Title = evaluation.Name,
            ActivityDate = evaluation.Date.Value,
            ActivityType = calendarType,
            HasReminder = true,
            ReminderDate = reminderDate,
            EvaluationId = evaluation.Id,
            CreatedDate = DateTime.Now
        });

        await _notificationService.SendActivityReminderAsync(
            student.UserId, created.Id, evaluation.Name, calendarType, reminderDate);
    }

    private async Task SyncCalendarUpdateAsync(int studentId, Evaluation evaluation, bool hasReminder)
    {
        var existing = await _calendarRepository.GetByEvaluationIdAsync(evaluation.Id);

        if (hasReminder && evaluation.Date.HasValue)
        {
            if (existing != null)
            {
                var calendarType = ValidCalendarTypes.Contains(evaluation.EvaluationType) ? evaluation.EvaluationType : "Otro";
                var reminderDate = evaluation.Date.Value.AddDays(-1);
                existing.Title = evaluation.Name;
                existing.ActivityDate = evaluation.Date.Value;
                existing.ActivityType = calendarType;
                existing.ReminderDate = reminderDate;
                await _calendarRepository.UpdateAsync(existing);
                await _notificationService.SendActivityReminderAsync(
                    existing.UserId, existing.Id, evaluation.Name, calendarType, reminderDate);
            }
            else
            {
                await SyncCalendarCreateAsync(studentId, evaluation.StudentProgress.CourseId, evaluation);
            }
        }
        else if (existing != null)
        {
            await _calendarRepository.DeleteAsync(existing.Id);
        }
    }

    private static EvaluationResponse MapToResponse(Evaluation e, bool hasReminder = false) => new()
    {
        Id = e.Id,
        Name = e.Name,
        EvaluationType = e.EvaluationType,
        Percentage = e.Percentage,
        Score = e.Score,
        Date = e.Date,
        HasReminder = hasReminder
    };

    private static string? ValidateEvaluationType(string evaluationType)
    {
        if (!ValidEvaluationTypes.Contains(evaluationType))
            return $"Tipo de evaluación inválido. Tipos permitidos: {string.Join(", ", ValidEvaluationTypes)}.";
        return null;
    }
}
