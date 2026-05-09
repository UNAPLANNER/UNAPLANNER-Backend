using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface INotesRepository
{
    Task<Student?> GetStudentByIdAsync(int studentId);
    Task<Student?> GetStudentByUserIdAsync(int userId);
    Task<List<Note>> GetNotesByUserIdAsync(int userId, int? courseId = null);
}
