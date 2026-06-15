using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface IUserStudentService
{
     // Deletes a student user after validating the current password
     Task<bool> DeleteUserStudent(int id, string currentPassword);
     Task<UpdateStudentResponseDto?> UpdateProfileStudent(int userId, UpdateStudentProfileRequest dto);
}