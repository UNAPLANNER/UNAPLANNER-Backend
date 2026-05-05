using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;
public interface IAuthRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<User> AddUser(User user);
    

}