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
            .Include(career => career.Campus)
            .Include(career => career.StudyPlans)
                .ThenInclude(studyPlan => studyPlan.StudyPlanCourses)
            .OrderBy(career => career.Name)
            .ToListAsync();
    }
}
