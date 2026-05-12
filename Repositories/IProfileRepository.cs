using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface IProfileRepository
{
    Task<Admin?> GetAdminByUserIdAsync(int userId);
    Task SaveChangesAsync();
}
