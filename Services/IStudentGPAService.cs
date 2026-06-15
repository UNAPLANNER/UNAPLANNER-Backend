using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface IStudentGPAService
{
    Task<StudentGPAResponse> GetGpaAsync(int studentId);
}