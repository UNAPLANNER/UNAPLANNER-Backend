using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface ICareerService
{
    Task<List<CareerResponse>> GetAllAsync();
}
