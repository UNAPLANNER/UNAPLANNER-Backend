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
                CourseName = n.Course?.Name ?? "General",
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

            // Si se asocia a un curso, validar que esté EnCurso para este estudiante
            if (request.CourseId.HasValue)
            {
                var isEnCurso = await _notesRepository.IsStudentCourseEnCursoAsync(studentId, request.CourseId.Value);
                if (!isEnCurso)
                {
                    return (false, null, "Solo puedes agregar notas a cursos que están en curso actualmente.");
                }
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
                CourseName = createdNote.Course?.Name ?? "General",
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

    public async Task<(bool Success, NoteResponse? Note, string? ErrorMessage)> UpdateNoteAsync(int noteId, int userId, UpdateNoteRequest request)
    {
        try
        {
            var note = await _notesRepository.GetNoteByIdAsync(noteId);
            if (note == null)
                return (false, null, $"Nota con ID {noteId} no encontrada.");

            if (note.UserId != userId)
                return (false, null, "No tienes permisos para modificar esta nota.");

            if (string.IsNullOrWhiteSpace(request.Title))
                return (false, null, "El título de la nota es requerido.");

            // Validar que el nuevo curso (si se cambia) sea EnCurso del estudiante
            if (request.CourseId.HasValue)
            {
                var student = await _notesRepository.GetStudentByUserIdAsync(userId);
                if (student == null)
                    return (false, null, "No se encontró el perfil de estudiante.");

                var isEnCurso = await _notesRepository.IsStudentCourseEnCursoAsync(student.StudentId, request.CourseId.Value);
                if (!isEnCurso)
                    return (false, null, "Solo puedes asociar notas a cursos que están en curso actualmente.");
            }

            note.Title = request.Title;
            note.Content = request.Content;
            note.CourseId = request.CourseId;
            note.LastUpdated = DateTime.Now;

            // Guardar los cambios en la base de datos
            var updatedNote = await _notesRepository.UpdateNoteAsync(note);

            if (updatedNote == null)
            {
                return (false, null, "Error al actualizar la nota en la base de datos.");
            }

            // Mapear a NoteResponse
            var noteResponse = new NoteResponse
            {
                Id = updatedNote.Id,
                Title = updatedNote.Title,
                Content = updatedNote.Content,
                CourseId = updatedNote.CourseId,
                CourseName = updatedNote.Course?.Name ?? "General",
                CreatedAt = updatedNote.CreatedAt,
                UpdatedAt = updatedNote.LastUpdated
            };

            return (true, noteResponse, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al actualizar la nota: {ex.Message}");
        }
    }

    public async Task<(bool Success, List<CourseResponse>? Courses, string? ErrorMessage)> GetStudentCoursesAsync(int studentId)
    {
        try
        {
            // Validar que el estudiante existe
            var student = await _notesRepository.GetStudentByIdAsync(studentId);
            if (student == null)
            {
                return (false, null, $"Estudiante con ID {studentId} no encontrado.");
            }

            // Obtener todos los cursos del plan de estudios del estudiante
            var courses = await _notesRepository.GetStudentStudyPlanCoursesAsync(studentId);

            // Mapear a CourseResponse
            var courseResponses = courses.Select(c => new CourseResponse
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Credits = c.Credits,
                TheoryHours = c.TheoryHours,
                PracticeHours = c.PracticeHours,
                LabHours = c.LabHours
            }).ToList();

            return (true, courseResponses, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al obtener los cursos: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteNoteAsync(int noteId, int userId)
    {
        try
        {
            // Validar que la nota existe
            var note = await _notesRepository.GetNoteByIdAsync(noteId);
            if (note == null)
            {
                return (false, $"Nota con ID {noteId} no encontrada.");
            }

            // Validar propiedad de la nota (seguridad)
            if (note.UserId != userId)
            {
                return (false, "No tienes permisos para eliminar esta nota.");
            }

            // Eliminar la nota
            var deleted = await _notesRepository.DeleteNoteAsync(noteId);

            if (!deleted)
            {
                return (false, "Error al eliminar la nota de la base de datos.");
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al eliminar la nota: {ex.Message}");
        }
    }
}

