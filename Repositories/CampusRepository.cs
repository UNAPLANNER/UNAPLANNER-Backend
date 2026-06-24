using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Models.Entities;

using UNAPLANNER_API.Data;



public class CampusRepository : ICampusRepository
{
    private readonly AppDbContext _context;
    public CampusRepository(AppDbContext context)
    {
        _context = context;
    }
    // Retrieves all campuses from the database without tracking changes,
    // improves performance using AsNoTracking, and returns the list of Campus entities asynchronously.
    public async Task<List<Campus>> GetAllCampus()
    {
        return await _context.Campuses
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Campus?> GetByIdCampus(int id)
    {
        return await _context.Campuses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}