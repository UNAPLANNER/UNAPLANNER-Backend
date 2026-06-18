using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface ICareerRepository
{
    Task<List<Career>> GetAllAsync();
    Task<List<Career>> GetByCampusIdAsync(int campusId);
    Task<Career?> GetByIdAsync(int id);
    Task<bool> ExistsByNameAsync(int campusId, string name);
    Task<bool> ExistsByCodeAsync(int campusId, string code);
    Task<Career> CreateAsync(Career career);
}
