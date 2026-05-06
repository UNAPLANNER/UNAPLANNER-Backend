using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface INotesService
{
    Task<(bool Success, List<NoteResponse>? Notes, string? ErrorMessage)> GetStudentNotesAsync(int studentId, int? courseId = null);
}
