using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;
public interface IUserStudentRepository
{
    // Retrieves a user with related student information by ID
    Task<User?> GetByIdUserStudent(int id);
    // Deletes a student user after validating the current password
    Task<bool> DeleteUserStudent(int id, string currentPassword);
    // Carrer Validation
    Task<bool> CareerExists(int careerId);
    // STUDENT (UPDATE PROFILE)
    Task<Student?> UpdateStudentProfile(Student student);
    Task<Student?> GetStudentByUserId(int userId);
}
