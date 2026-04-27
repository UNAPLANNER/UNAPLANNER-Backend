using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == id);
    }
    public async Task DeleteUser(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }


}
