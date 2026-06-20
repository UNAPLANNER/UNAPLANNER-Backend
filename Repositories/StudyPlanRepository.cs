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

        return await GetRequiredDetailAsync(studyPlan.StudyPlanId);
    }

    public async Task<bool> CourseCodeExistsAsync(string code)
    {
        return await _context.Courses
            .AsNoTracking()
            .AnyAsync(course => course.Code.ToLower() == code.ToLower());
    }

    public async Task<bool> CourseCodeExistsExcludingCourseAsync(string code, int courseId)
    {
        return await _context.Courses
            .AsNoTracking()
            .AnyAsync(course =>
                course.Id != courseId &&
                course.Code.ToLower() == code.ToLower());
    }

    public async Task<bool> StudyPlanExistsAsync(int studyPlanId)
    {
        return await _context.StudyPlans
            .AsNoTracking()
            .AnyAsync(studyPlan => studyPlan.StudyPlanId == studyPlanId && studyPlan.IsStatus);
    }

    public async Task<bool> StudyPlanContainsCoursesAsync(int studyPlanId, List<int> courseIds)
    {
        if (courseIds.Count == 0)
            return true;

        var distinctCourseIds = courseIds.Distinct().ToList();
        var matchingCourseCount = await _context.StudyPlanCourses
            .AsNoTracking()
            .Where(planCourse =>
                planCourse.StudyPlanId == studyPlanId &&
                planCourse.IsStatus &&
                distinctCourseIds.Contains(planCourse.CourseId))
            .Select(planCourse => planCourse.CourseId)
            .Distinct()
            .CountAsync();

        return matchingCourseCount == distinctCourseIds.Count;
    }

    public async Task<bool> StudyPlanCourseExistsAsync(int studyPlanId, int courseId)
    {
        return await _context.StudyPlanCourses
            .AsNoTracking()
            .AnyAsync(planCourse =>
                planCourse.StudyPlanId == studyPlanId &&
                planCourse.CourseId == courseId);
    }

    public async Task<StudyPlanCourse?> GetStudyPlanCourseForUpdateAsync(int studyPlanId, int courseId)
    {
        return await _context.StudyPlanCourses
            .Include(planCourse => planCourse.Course)
            .FirstOrDefaultAsync(planCourse =>
                planCourse.StudyPlanId == studyPlanId &&
                planCourse.CourseId == courseId);
    }

    public async Task<StudyPlan> CreateCourseAsync(
        int studyPlanId,
        Course course,
        StudyPlanCourse studyPlanCourse,
        List<Requirement> requirements)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        studyPlanCourse.CourseId = course.Id;
        _context.StudyPlanCourses.Add(studyPlanCourse);

        foreach (var requirement in requirements)
        {
            requirement.CourseId = course.Id;
            _context.Requirements.Add(requirement);
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return await GetRequiredDetailAsync(studyPlanId);
    }

    public async Task<StudyPlan> UpdateCourseAsync(
        int studyPlanId,
        int courseId,
        Course course,
        StudyPlanCourse studyPlanCourse,
        List<Requirement> requirements)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        _context.Courses.Update(course);
        _context.StudyPlanCourses.Update(studyPlanCourse);

        var existingRequirements = await _context.Requirements
            .Where(requirement => requirement.CourseId == courseId)
            .ToListAsync();

        _context.Requirements.RemoveRange(existingRequirements);
        _context.Requirements.AddRange(requirements);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return await GetRequiredDetailAsync(studyPlanId);
    }

    private async Task<StudyPlan> GetRequiredDetailAsync(int studyPlanId)
    {
        return await _context.StudyPlans
            .AsNoTracking()
            .Include(plan => plan.Career)
            .Include(plan => plan.StudyPlanCourses.Where(planCourse => planCourse.IsStatus))
                .ThenInclude(planCourse => planCourse.Course)
                    .ThenInclude(course => course.CourseRequirements)
                        .ThenInclude(requirement => requirement.RequiredCourse)
            .FirstAsync(plan => plan.StudyPlanId == studyPlanId);
    }
}
