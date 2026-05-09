using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.Models.Entities;
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

    public async Task<(bool Success, NoteResponse? Note, string? ErrorMessage)> CreateNoteByUserIdAsync(int userId, CreateNoteRequest request)
    {
        try
        {
            var student = await _notesRepository.GetStudentByUserIdAsync(userId);
            if (student == null)
            {
                return (false, null, $"Estudiante asociado al usuario {userId} no encontrado.");
            }

            var note = new Note
            {
                UserId = userId,
                CourseId = request.CourseId,
                Title = request.Title.Trim(),
                Content = request.Content,
                CreatedAt = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            var created = await _notesRepository.AddNoteAsync(note);
            return (true, MapNote(created), null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al crear la nota: {ex.Message}");
        }
    }

    public async Task<(bool Success, NoteResponse? Note, string? ErrorMessage)> UpdateNoteAsync(int noteId, UpdateNoteRequest request)
    {
        try
        {
            var note = await _notesRepository.GetNoteByIdAsync(noteId);
            if (note == null)
            {
                return (false, null, $"Nota con ID {noteId} no encontrada.");
            }

            note.Title = request.Title.Trim();
            note.Content = request.Content;
            note.CourseId = request.CourseId;
            note.LastUpdated = DateTime.Now;

            await _notesRepository.UpdateNoteAsync(note);
            await _notesRepository.SaveChangesAsync();

            return (true, MapNote(note), null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al actualizar la nota: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteNoteAsync(int noteId, int userId)
    {
        try
        {
            var note = await _notesRepository.GetNoteByIdAsync(noteId);
            if (note == null)
            {
                return (false, $"Nota con ID {noteId} no encontrada.");
            }

            if (note.UserId != userId)
            {
                return (false, "No tienes permisos para eliminar esta nota.");
            }

            await _notesRepository.DeleteNoteAsync(note);
            await _notesRepository.SaveChangesAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al eliminar la nota: {ex.Message}");
        }
    }

    private static NoteResponse MapNote(Note note)
    {
        return new NoteResponse
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CourseId = note.CourseId,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.LastUpdated
        };
    }
}
