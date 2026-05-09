using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class NotesService : INotesService
{
    private readonly INotesRepository _notesRepository;

    public NotesService(INotesRepository notesRepository)
    {
        _notesRepository = notesRepository;
    }

    public async Task<(bool Success, List<NoteResponse>? Notes, string? ErrorMessage)> GetStudentNotesAsync(int studentId, int? courseId = null)
    {
        try
        {
            // Validar que el estudiante existe
            var student = await _notesRepository.GetStudentByIdAsync(studentId);
            if (student == null)
            {
                return (false, null, $"Estudiante con ID {studentId} no encontrado.");
            }

            // Obtener las notas del usuario (student.UserId)
            var notes = await _notesRepository.GetNotesByUserIdAsync(student.UserId, courseId);

            // Mapear a NoteResponse
            var noteResponses = notes.Select(n => new NoteResponse
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                CourseId = n.CourseId,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.LastUpdated
            }).ToList();

            return (true, noteResponses, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al obtener las notas: {ex.Message}");
        }
    }

    public async Task<(bool Success, List<NoteResponse>? Notes, string? ErrorMessage)> GetStudentNotesByUserIdAsync(int userId, int? courseId = null)
    {
        try
        {
            var student = await _notesRepository.GetStudentByUserIdAsync(userId);
            if (student == null)
            {
                return (false, null, $"Estudiante asociado al usuario {userId} no encontrado.");
            }

            var notes = await _notesRepository.GetNotesByUserIdAsync(userId, courseId);
            var noteResponses = notes.Select(n => new NoteResponse
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                CourseId = n.CourseId,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.LastUpdated
            }).ToList();

            return (true, noteResponses, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al obtener las notas: {ex.Message}");
        }
    }
}
