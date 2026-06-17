using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Mappers;

public class CurriculumMapper
{
    public static CurriculumCourseResponse ToCurriculumCourseResponse(
        StudyPlanCourse spc,
        StudentProgress? progress = null)
    {
        return new CurriculumCourseResponse
        {
            Id = spc.Course.Id,
            Code = spc.Course.Code,
            Name = spc.Course.Name,
            Credits = spc.Course.Credits,
            TheoryHours = spc.Course.TheoryHours,
            PracticeHours = spc.Course.PracticeHours,
            LabHours = spc.Course.LabHours,
            IsElective = spc.IsElective,
            ElectiveType = spc.ElectiveType,
            Status = progress?.Status ?? "Pendiente",
            FinalGrade = progress?.FinalGrade
        };
    }

    public static List<LevelCurriculumResponse> ToLevelCurriculumResponseList(
        List<StudyPlanCourse> studyPlanCourses,
        List<StudentProgress> progressList)
    {
        var progressDict = progressList.ToDictionary(p => p.CourseId);

        return studyPlanCourses
            .GroupBy(spc => spc.Levels)
            .OrderBy(g => g.Key)
            .Select(levelGroup => new LevelCurriculumResponse
            {
                Level = levelGroup.Key,
                Semesters = levelGroup
                    .GroupBy(spc => spc.Term)
                    .OrderBy(g => g.Key)
                    .Select(termGroup => new SemesterCoursesResponse
                    {
                        Semester = termGroup.Key,
                        Courses = termGroup
                            .OrderBy(spc => spc.Course.Name)
                            .Select(spc =>
                            {
                                progressDict.TryGetValue(spc.CourseId, out var progress);
                                return ToCurriculumCourseResponse(spc, progress);
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToList();
    }

    public static StudentCourseProgressResponse ToStudentCourseProgressResponse(
        StudyPlanCourse spc,
        StudentProgress? progress)
    {
        return new StudentCourseProgressResponse
        {
            CourseId = spc.Course.Id,
            Code = spc.Course.Code,
            Name = spc.Course.Name,
            Credits = spc.Course.Credits,
            IsElective = spc.IsElective,
            ElectiveType = spc.ElectiveType,
            Level = spc.Levels,
            Term = spc.Term,
            Status = progress?.Status ?? "Pendiente",
            FinalGrade = progress?.FinalGrade,
            Semester = progress?.AcademicTerm,
            Year = progress?.TermYear
        };
    }

    public static List<StudentCourseProgressResponse> ToStudentCourseProgressList(
        List<StudyPlanCourse> studyPlanCourses,
        List<StudentProgress> progressList)
    {
        var progressDict = progressList.ToDictionary(p => p.CourseId);

        return studyPlanCourses
            .OrderBy(spc => spc.Levels)
            .ThenBy(spc => spc.Term)
            .ThenBy(spc => spc.Course.Name)
            .Select(spc =>
            {
                progressDict.TryGetValue(spc.CourseId, out var progress);
                return ToStudentCourseProgressResponse(spc, progress);
            })
            .ToList();
    }
}
