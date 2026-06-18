using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface ICareerRepository
{
    Task<List<Career>> GetAllAsync();
    Task<List<Career>> GetByCampusIdAsync(int campusId);
    Task<Career?> GetByIdAsync(int id);
    Task<Career?> GetEditableByIdAsync(int id);
    Task<bool> ExistsByCodeExcludingIdAsync(int careerId, string code);
    Task<Career> UpdateAsync(Career career);
}
