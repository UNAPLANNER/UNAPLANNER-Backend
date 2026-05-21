using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class CareerService : ICareerService
{
    private readonly ICareerRepository _careerRepository;

    public CareerService(ICareerRepository careerRepository)
    {
        _careerRepository = careerRepository;
    }

    public async Task<List<CareerResponse>> GetAllAsync()
    {
        var careers = await _careerRepository.GetAllAsync();
        return CareerMapper.ToResponseList(careers);
    }
}
