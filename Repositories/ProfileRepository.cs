using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly AppDbContext _context;

    public ProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Admin)
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
