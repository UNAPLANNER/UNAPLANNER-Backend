using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.DTOs.Requests;

namespace UNAPLANNER_API.Services;

public interface ICampusService
{
    // Gets all registered campuses from the system.
    // Returns a list of CampusResponseDto containing campus details.
    Task<List<CampusResponseDto>> GetAllCampus();

    // Retrieves a campus by its ID and returns it as a DTO.
    Task<CampusResponseDto?> GetCampusByIdAsync(int id);

    // Creates a new campus with validation
    Task<(bool Success, string Error, CampusResponseDto? Campus)> CreateCampusAsync(CreateCampusRequest request);
}