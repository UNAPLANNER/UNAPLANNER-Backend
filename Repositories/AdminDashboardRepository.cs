using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Repositories;

public class AdminDashboardRepository : IAdminDashboardRepository
{
    private readonly AppDbContext _context;

    public AdminDashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardResponse?> GetDashboardByAdminUserIdAsync(int userId)
    {
        var admin = await _context.Admins
            .AsNoTracking()
            .Include(admin => admin.Campus)
            .FirstOrDefaultAsync(admin => admin.UserId == userId);

        if (admin == null)
            return null;

        var careers = await _context.Careers
            .AsNoTracking()
            .Include(career => career.StudyPlans)
                .ThenInclude(studyPlan => studyPlan.StudyPlanCourses)
            .Where(career => career.CampusId == admin.CampusId)
            .OrderBy(career => career.Name)
            .ToListAsync();

        var activeCareerIds = careers
            .Where(career => career.IsStatus)
            .Select(career => career.Id)
            .ToList();

        var activeStudyPlanIds = careers
            .SelectMany(career => career.StudyPlans)
            .Where(studyPlan => studyPlan.IsStatus)
            .Select(studyPlan => studyPlan.StudyPlanId)
            .Distinct()
            .ToList();

        var registeredCourses = await _context.StudyPlanCourses
            .AsNoTracking()
            .Where(planCourse => planCourse.IsStatus && activeStudyPlanIds.Contains(planCourse.StudyPlanId))
            .Select(planCourse => planCourse.CourseId)
            .Distinct()
            .CountAsync();

        var activeStudents = await _context.Students
            .AsNoTracking()
            .CountAsync(student => student.IsStatus && activeCareerIds.Contains(student.CareerId));

        var careerResponses = careers
            .Select(ToCareerResponse)
            .ToList();

        return new AdminDashboardResponse
        {
            AdminName = admin.FullName,
            Department = admin.Department,
            CampusId = admin.CampusId,
            CampusName = admin.Campus.Name,
            ActiveCareers = careers.Count(career => career.IsStatus),
            StudyPlans = activeStudyPlanIds.Count,
            RegisteredCourses = registeredCourses,
            ActiveStudents = activeStudents,
            LatestCareer = careerResponses
                .OrderByDescending(career => career.CreatedDate)
                .FirstOrDefault(),
            Careers = careerResponses
        };
    }

    private static AdminDashboardCareerResponse ToCareerResponse(Models.Entities.Career career)
    {
        var currentStudyPlan = career.StudyPlans
            .Where(studyPlan => studyPlan.IsStatus)
            .OrderByDescending(studyPlan => studyPlan.ValidYear)
            .ThenByDescending(studyPlan => studyPlan.CreatedDate)
            .FirstOrDefault();

        var courseCount = currentStudyPlan?.StudyPlanCourses
            .Where(planCourse => planCourse.IsStatus)
            .Select(planCourse => planCourse.CourseId)
            .Distinct()
            .Count() ?? 0;

        return new AdminDashboardCareerResponse
        {
            Id = career.Id,
            Name = career.Name,
            Code = career.Code,
            TotalCredits = career.TotalCredits,
            CourseCount = courseCount,
            StudyPlanId = currentStudyPlan?.StudyPlanId,
            StudyPlanYear = currentStudyPlan?.ValidYear,
            IsStatus = career.IsStatus,
            CreatedDate = career.CreatedDate
        };
    }
}
