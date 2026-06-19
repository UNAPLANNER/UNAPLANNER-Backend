using UNAPLANNER_API.Models.Entities;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Repositories;
using UNAPLANNER_API.Services;
using UNAPLANNER_API.Mappers;

public class CampusService : ICampusService
{
    private readonly ICampusRepository _repository;

    public CampusService(ICampusRepository repository)
    {
        _repository = repository;
    }
    // Retrieves all campuses from the repository asynchronously,
    // checks if the result is null or empty, and returns an empty list if no data is found.
    // Otherwise, maps the Campus entities to CampusResponseDto objects using the CampusMapper.
    public async Task<List<CampusResponseDto>> GetAllCampus()
    {
        var campuses = await _repository.GetAllCampus();

        if (campuses == null || campuses.Count == 0)
            return new List<CampusResponseDto>();

        return CampusMapper.ToDtoList(campuses);
    }
     // Retrieves a campus by its ID and returns it as a DTO.
    public async Task<CampusResponseDto?> GetCampusByIdAsync(int id)
    {
        var campus = await _repository.GetByIdCampus(id);

        if (campus == null)
            return null;

        return CampusMapper.ToDto(campus);
    }
    public async Task<(bool Success, string Error, CampusResponseDto? Campus)> CreateCampusAsync(CreateCampusRequest request)
    {
        var existing = await _repository.GetByCodeCampus(request.Code);
        if (existing != null)
        {
            return (false, "El código ya existe", null);
        }

        var campus = CampusMapper.ToEntity(request);

        var created = await _repository.CreateCampusAsync(campus);

        var response = CampusMapper.ToDto(created);

        return (true, string.Empty, response);

    }
}