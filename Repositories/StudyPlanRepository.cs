using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public class StudyPlanRepository : IStudyPlanRepository
{
    private readonly AppDbContext _context;

    public StudyPlanRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StudyPlan?> GetDetailByIdAsync(int studyPlanId)
    {
        return await _context.StudyPlans
            .Include(studyPlan => studyPlan.Career)
            .Include(studyPlan => studyPlan.StudyPlanCourses.Where(planCourse => planCourse.IsStatus))
                .ThenInclude(planCourse => planCourse.Course)
                    .ThenInclude(course => course.CourseRequirements)
                        .ThenInclude(requirement => requirement.RequiredCourse)
            .FirstOrDefaultAsync(studyPlan => studyPlan.StudyPlanId == studyPlanId && studyPlan.IsStatus);
    }

    public async Task<bool> CareerExistsAsync(int careerId)
    {
        return await _context.Careers
            .AsNoTracking()
            .AnyAsync(career => career.Id == careerId);
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _context.StudyPlans
            .AsNoTracking()
            .AnyAsync(studyPlan => studyPlan.Code.ToLower() == code.ToLower());
    }

    public async Task<StudyPlan> CreateAsync(StudyPlan studyPlan)
    {
        _context.StudyPlans.Add(studyPlan);
        await _context.SaveChangesAsync();

        return await _context.StudyPlans
            .AsNoTracking()
            .Include(plan => plan.Career)
            .Include(plan => plan.StudyPlanCourses.Where(planCourse => planCourse.IsStatus))
                .ThenInclude(planCourse => planCourse.Course)
                    .ThenInclude(course => course.CourseRequirements)
                        .ThenInclude(requirement => requirement.RequiredCourse)
            .FirstAsync(plan => plan.StudyPlanId == studyPlan.StudyPlanId);
    }
}
