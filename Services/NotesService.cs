using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.Repositories;
using UNAPLANNER_API.Models.Entities;

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

    public async Task<(bool Success, NoteResponse? Note, string? ErrorMessage)> CreateNoteAsync(int studentId, CreateNoteRequest request)
    {
        try
        {
            // Validar que el estudiante existe
            var student = await _notesRepository.GetStudentByIdAsync(studentId);
            if (student == null)
            {
                return (false, null, $"Estudiante con ID {studentId} no encontrado.");
            }

            // Validar que el título no esté vacío
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return (false, null, "El título de la nota es requerido.");
            }

            // Crear la nota asociada al usuario del estudiante
            var note = new Note
            {
                UserId = student.UserId,
                Title = request.Title,
                Content = request.Content,
                CourseId = request.CourseId,
                CreatedAt = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            // Guardar la nota en la base de datos
            var createdNote = await _notesRepository.CreateNoteAsync(note);

            if (createdNote == null)
            {
                return (false, null, "Error al crear la nota en la base de datos.");
            }

            // Mapear a NoteResponse
            var noteResponse = new NoteResponse
            {
                Id = createdNote.Id,
                Title = createdNote.Title,
                Content = createdNote.Content,
                CourseId = createdNote.CourseId,
                CreatedAt = createdNote.CreatedAt,
                UpdatedAt = createdNote.LastUpdated
            };

            return (true, noteResponse, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al crear la nota: {ex.Message}");
        }
    }
}

