using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Constants;
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

    public async Task<Admin?> GetAdminByUserIdAsync(int userId)
    {
        return await _context.Admins
            .Include(a => a.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(a => a.UserId == userId && a.User.RoleId == RoleContants.Admin);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
