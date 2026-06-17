using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface IUserStudentService
{
     // Deletes a student user after validating the current password
     Task<bool> DeleteUserStudent(int id, string currentPassword);
     // Updates the profile information for the student associated with the user
     Task<UpdateStudentResponseDto?> UpdateProfileStudent(int userId, UpdateStudentProfileRequest dto);
     /// Retrieves the student profile information associated with the user
     Task<UpdateStudentResponseDto?> GetStudentProfile(int userId);
}