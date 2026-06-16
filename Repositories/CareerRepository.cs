using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public class CareerRepository : ICareerRepository
{
    private readonly AppDbContext _context;

    public CareerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Career>> GetAllAsync()
    {
        return await _context.Careers
            .Where(c => c.IsStatus)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Career?> GetByIdAsync(int id)
    {
        return await _context.Careers
            .FirstOrDefaultAsync(c => c.Id == id && c.IsStatus);
    }
}
