using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface IProfileRepository
{
    Task<User?> GetByIdAsync(int userId);
    Task UpdateAsync(User user);
    Task<bool> SaveChangesAsync();
}
