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
}
