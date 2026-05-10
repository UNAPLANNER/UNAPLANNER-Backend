using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;
public interface IUserStudentRepository
{
    Task<User?> GetByIdUserStudent(int id);
    Task<bool> DeleteUserStudent(int id, string currentPassword);
}
