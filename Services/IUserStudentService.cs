using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface IUserStudentService
{
     Task<bool> DeleteUserStudent(int id, string currentPassword);
}