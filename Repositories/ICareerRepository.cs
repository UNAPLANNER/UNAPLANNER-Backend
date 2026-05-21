using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface ICareerRepository
{
    Task<List<Career>> GetAllAsync();
}
