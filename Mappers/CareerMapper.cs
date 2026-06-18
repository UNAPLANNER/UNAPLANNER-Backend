using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Mappers;

public class CareerMapper
{
    public static CareerResponse ToResponse(Career career)
    {
        var currentStudyPlan = career.StudyPlans
            .Where(studyPlan => studyPlan.IsStatus)
            .OrderByDescending(studyPlan => studyPlan.ValidYear)
            .ThenByDescending(studyPlan => studyPlan.CreatedDate)
            .FirstOrDefault();

        var activePlanCourses = currentStudyPlan?.StudyPlanCourses
            .Where(studyPlanCourse => studyPlanCourse.IsStatus)
            .ToList() ?? [];

        return new CareerResponse
        {
            Id = career.Id,
            CampusId = career.CampusId,
            CampusName = career.Campus?.Name ?? "N/A",
            Name = career.Name,
            Code = career.Code,
            Description = career.Description,
            TotalCredits = career.TotalCredits,
            CurrentStudyPlanId = currentStudyPlan?.StudyPlanId,
            CurrentStudyPlanName = currentStudyPlan?.Name,
            CurrentStudyPlanYear = currentStudyPlan?.ValidYear,
            CourseCount = activePlanCourses.Count,
            LevelCount = activePlanCourses.Select(studyPlanCourse => studyPlanCourse.Levels).Distinct().Count(),
            IsStatus = career.IsStatus,
            CreatedDate = career.CreatedDate
        };
    }

    public static List<CareerResponse> ToResponseList(List<Career> careers)
    {
        return careers.Select(ToResponse).ToList();
    }
}
