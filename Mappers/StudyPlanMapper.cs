using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Mappers;

public static class StudyPlanMapper
{
    public static StudyPlanDetailResponse ToDetailResponse(StudyPlan studyPlan)
    {
        var activeCourses = studyPlan.StudyPlanCourses
            .Where(planCourse => planCourse.IsStatus)
            .OrderBy(planCourse => planCourse.Levels)
            .ThenBy(planCourse => planCourse.Term)
            .ThenBy(planCourse => planCourse.Course.Name)
            .ToList();
        var creditCourses = GetCoursesForCreditTotals(activeCourses);

        var levels = activeCourses
            .GroupBy(planCourse => planCourse.Levels)
            .OrderBy(levelGroup => levelGroup.Key)
            .Select(levelGroup => new StudyPlanLevelResponse
            {
                Level = levelGroup.Key,
                Credits = levelGroup
                    .Where(planCourse => creditCourses.Contains(planCourse.Id))
                    .Sum(planCourse => planCourse.Course.Credits),
                Semesters = levelGroup
                    .GroupBy(planCourse => planCourse.Term)
                    .OrderBy(semesterGroup => semesterGroup.Key)
                    .Select(semesterGroup => new StudyPlanSemesterResponse
                    {
                        Semester = semesterGroup.Key,
                        Courses = semesterGroup
                            .OrderBy(planCourse => planCourse.Course.Name)
                            .Select(ToCourseDetailResponse)
                            .ToList()
                    })
                    .ToList()
            })
            .ToList();

        return new StudyPlanDetailResponse
        {
            Id = studyPlan.StudyPlanId,
            Name = studyPlan.Name,
            Code = studyPlan.Code,
            CareerId = studyPlan.CareerId,
            CareerName = studyPlan.Career.Name,
            CareerCode = studyPlan.Career.Code,
            EffectiveYear = studyPlan.ValidYear,
            TotalCredits = activeCourses
                .Where(planCourse => creditCourses.Contains(planCourse.Id))
                .Sum(planCourse => planCourse.Course.Credits),
            CourseCount = activeCourses.Select(planCourse => planCourse.CourseId).Distinct().Count(),
            LevelCount = levels.Count,
            CycleCount = activeCourses.Select(planCourse => planCourse.Term).Distinct().Count(),
            Levels = levels
        };
    }

    private static HashSet<int> GetCoursesForCreditTotals(List<StudyPlanCourse> studyPlanCourses)
    {
        var mandatoryCourseIds = studyPlanCourses
            .Where(planCourse => !planCourse.IsElective)
            .Select(planCourse => planCourse.Id);

        var electiveCourseIds = studyPlanCourses
            .Where(planCourse => planCourse.IsElective)
            .GroupBy(planCourse => new { planCourse.Levels, planCourse.Term })
            .OrderByDescending(levelGroup => levelGroup.Key.Levels)
            .ThenBy(levelGroup => levelGroup.Key.Term)
            .SelectMany(levelGroup => levelGroup
                .OrderBy(planCourse => planCourse.Course.Name)
                .Take(2))
            .Take(4)
            .Select(planCourse => planCourse.Id);

        return mandatoryCourseIds
            .Concat(electiveCourseIds)
            .ToHashSet();
    }

    private static StudyPlanCourseDetailResponse ToCourseDetailResponse(StudyPlanCourse planCourse)
    {
        return new StudyPlanCourseDetailResponse
        {
            Id = planCourse.Course.Id,
            Code = planCourse.Course.Code,
            Name = planCourse.Course.Name,
            Credits = planCourse.Course.Credits,
            TheoryHours = planCourse.Course.TheoryHours,
            PracticeHours = planCourse.Course.PracticeHours,
            LabHours = planCourse.Course.LabHours,
            IsElective = planCourse.IsElective,
            ElectiveType = planCourse.ElectiveType,
            Prerequisites = planCourse.Course.CourseRequirements
                .Where(requirement => requirement.RequirementType == "Prerequisite")
                .OrderBy(requirement => requirement.RequiredCourse.Code)
                .Select(requirement => new StudyPlanCourseRequirementResponse
                {
                    CourseId = requirement.RequiredCourseId,
                    Code = requirement.RequiredCourse.Code,
                    Name = requirement.RequiredCourse.Name,
                    RequirementType = requirement.RequirementType
                })
                .ToList()
        };
    }
}
