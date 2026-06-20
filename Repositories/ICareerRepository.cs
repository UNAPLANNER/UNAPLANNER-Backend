using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface ICareerRepository
{
    Task<List<Career>> GetAllAsync();
    Task<List<Career>> GetByCampusIdAsync(int campusId);
    Task<List<Career>> GetActiveByCampusIdAsync(int campusId);
    Task<Career?> GetByIdAsync(int id);
    Task<Career?> GetEditableByIdAsync(int id);
    Task<bool> ExistsByCodeExcludingIdAsync(int careerId, string code);
    Task<Career> UpdateAsync(Career career);
    Task<bool> ExistsByNameAsync(int campusId, string name);
    Task<bool> ExistsByCodeAsync(int campusId, string code);
    Task<Career> CreateAsync(Career career);
}
