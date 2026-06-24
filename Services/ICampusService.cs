using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface ICampusService
{
    // Gets all registered campuses from the system.
    // Returns a list of CampusResponseDto containing campus details.
    Task<List<CampusResponseDto>> GetAllCampus();

    // Retrieves a campus by its ID and returns it as a DTO.
    Task<CampusResponseDto?> GetCampusByIdAsync(int id);
}