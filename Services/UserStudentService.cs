using System.Text;
using Microsoft.IdentityModel.Tokens;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Repositories;
using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.Mappers;

namespace UNAPLANNER_API.Services;

public class UserStudentService : IUserStudentService
{
    private readonly IUserStudentRepository _userStudentRepository;

    public UserStudentService(IUserStudentRepository userStudentRepository)
    {
        _userStudentRepository = userStudentRepository;
    }

    public async Task<bool> DeleteUserStudent(int id, string currentPassword)
    {
        // Retrieves the user by ID
        var user = await _userStudentRepository.GetByIdUserStudent(id);
        if (user == null) return false;

        // Verifies if the provided password matches the stored password
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(currentPassword, user.Password);
        if (!isPasswordValid) return false;

        return await _userStudentRepository.DeleteUserStudent(id, currentPassword);
    }
    /**Validates the UpdateStudentProfileRequest object, The request data to validate.
    Returns true if the data is valid; otherwise false**/
    public static bool IsValid(UpdateStudentProfileRequest dto)
    {
        if (dto == null)
            return false;

        if (string.IsNullOrWhiteSpace(dto.FullName))
            return false;

        if (dto.CareerId <= 0)
            return false;

        return true;
    }
    public async Task<UpdateStudentResponseDto?> UpdateProfileStudent(int userId, UpdateStudentProfileRequest dto)
    {
        // Verify that the DTO contains valid data non-empty name, valid careerId
        if (!IsValid(dto))
            return null;
        // Searches the database for the student associated with the userId
        var student = await _userStudentRepository.GetStudentByUserId(userId);
        // Check if the degree program (CareerId) exists in the database
        if (student == null)
            return null;
        // If the degree program does not exist, the operation is canceled
        var careerExists = await _userStudentRepository.CareerExists(dto.CareerId);
        // Update the student's fields with the data from the DTO
        if (!careerExists)
            return null;

        StudentMapper.UpdateEntity(student, dto);

        var updatedStudent = await _userStudentRepository.UpdateStudentProfile(student);
        // If the update fails, null is returned
        if (updatedStudent == null)
            return null;
        // Convert the updated entity to a response DTO
        return StudentMapper.ToResponse(updatedStudent);
    }

    /** Retrieves the student profile associated with the authenticated user and converts it to a response DTO, “userId” (user identifier)
    An UpdateStudentResponseDto object containing the student's information,
    or null if there is no student associated with the user**/

    public async Task<UpdateStudentResponseDto?> GetStudentProfile(int userId)
    {
        // Reutiliza el mismo método del repository que ya existe
        var student = await _userStudentRepository.GetStudentByUserIdWithUser(userId);

        if (student == null) return null;

        return StudentMapper.ToResponse(student);
    }
}

