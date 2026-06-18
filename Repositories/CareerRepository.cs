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
            .AsNoTracking()
            .Include(c => c.Campus)
            .Include(c => c.StudyPlans)
                .ThenInclude(sp => sp.StudyPlanCourses)
            .Where(c => c.IsStatus)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<Career>> GetByCampusIdAsync(int campusId)
    {
        return await _context.Careers
            .AsNoTracking()
            .Include(c => c.Campus)
            .Include(c => c.StudyPlans)
                .ThenInclude(sp => sp.StudyPlanCourses)
            .Where(c => c.CampusId == campusId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Career?> GetByIdAsync(int id)
    {
        return await _context.Careers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.IsStatus);
    }

    public async Task<bool> ExistsByNameAsync(int campusId, string name)
    {
        return await _context.Careers
            .AsNoTracking()
            .AnyAsync(c => c.CampusId == campusId && c.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> ExistsByCodeAsync(int campusId, string code)
    {
        return await _context.Careers
            .AsNoTracking()
            .AnyAsync(c => c.CampusId == campusId && c.Code.ToLower() == code.ToLower());
    }

    public async Task<Career> CreateAsync(Career career)
    {
        _context.Careers.Add(career);
        await _context.SaveChangesAsync();

        return await _context.Careers
            .AsNoTracking()
            .Include(c => c.Campus)
            .Include(c => c.StudyPlans)
                .ThenInclude(sp => sp.StudyPlanCourses)
            .FirstAsync(c => c.Id == career.Id);
    }
}
