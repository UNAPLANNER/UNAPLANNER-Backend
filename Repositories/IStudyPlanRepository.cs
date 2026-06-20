using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface IStudyPlanRepository
{
    Task<StudyPlan?> GetDetailByIdAsync(int studyPlanId);
    Task<bool> CareerExistsAsync(int careerId);
    Task<bool> ExistsByCodeAsync(string code);
    Task<StudyPlan> CreateAsync(StudyPlan studyPlan);
    Task<bool> CourseCodeExistsAsync(string code);
    Task<bool> StudyPlanExistsAsync(int studyPlanId);
    Task<bool> StudyPlanContainsCoursesAsync(int studyPlanId, List<int> courseIds);
    Task<StudyPlan> CreateCourseAsync(
        int studyPlanId,
        Course course,
        StudyPlanCourse studyPlanCourse,
        List<Requirement> requirements);
    Task<bool> CourseCodeExistsExcludingCourseAsync(string code, int courseId);
    Task<bool> StudyPlanCourseExistsAsync(int studyPlanId, int courseId);
    Task<StudyPlanCourse?> GetStudyPlanCourseForUpdateAsync(int studyPlanId, int courseId);
    Task<StudyPlan> UpdateCourseAsync(
        int studyPlanId,
        int courseId,
        Course course,
        StudyPlanCourse studyPlanCourse,
        List<Requirement> requirements);
    Task<StudyPlan> DeleteCourseAsync(int studyPlanId, int courseId);
}
