using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.DTOs.Requests;

namespace UNAPLANNER_API.Services;

public interface ICareerService
{
    Task<List<CareerResponse>> GetAllCareersAsync();
    Task<List<CareerResponse>> GetCareersByAdminUserIdAsync(int userId);
    Task<CareerResponse> UpdateCareerForAdminAsync(int userId, int careerId, UpdateCareerRequest request);
    Task<CareerResponse> CreateCareerForAdminAsync(int userId, CreateCareerRequest request);
    Task<List<LevelCurriculumResponse>> GetCurriculumByCareerIdAsync(int careerId, int? userId = null);
}
