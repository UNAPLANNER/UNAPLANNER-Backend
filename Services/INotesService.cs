using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.DTOs.Requests;

namespace UNAPLANNER_API.Services;

public interface INotesService
{
    Task<(bool Success, List<NoteResponse>? Notes, string? ErrorMessage)> GetStudentNotesAsync(int studentId, int? courseId = null);
    Task<(bool Success, List<NoteResponse>? Notes, string? ErrorMessage)> GetStudentNotesByUserIdAsync(int userId, int? courseId = null);
    Task<(bool Success, NoteResponse? Note, string? ErrorMessage)> CreateNoteByUserIdAsync(int userId, CreateNoteRequest request);
    Task<(bool Success, NoteResponse? Note, string? ErrorMessage)> UpdateNoteAsync(int noteId, UpdateNoteRequest request);
    Task<(bool Success, string? ErrorMessage)> DeleteNoteAsync(int noteId, int userId);
}
