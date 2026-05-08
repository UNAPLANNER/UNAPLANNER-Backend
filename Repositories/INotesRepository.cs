using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface INotesRepository
{
    Task<Student?> GetStudentByIdAsync(int studentId);
    Task<Note?> GetNoteByIdAsync(int noteId);
    Task<List<Note>> GetNotesByUserIdAsync(int userId, int? courseId = null);
    Task<Note?> CreateNoteAsync(Note note);
    Task<Note?> UpdateNoteAsync(Note note);
    Task<List<Course>> GetStudentStudyPlanCoursesAsync(int studentId);
}
