using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface ICurriculumRepository
{
    Task<Student?> GetStudentByIdAsync(int studentId);
    Task<Student?> GetStudentByUserIdAsync(int userId);
    Task<List<StudyPlanCourse>> GetStudyPlanCoursesAsync(int studyPlanId);
    Task<List<StudentProgress>> GetStudentProgressAsync(int studentId);
    Task<StudentProgress?> GetStudentProgressForCourseAsync(int studentId, int courseId);
    Task<StudentProgress> UpsertStudentProgressAsync(int studentId, int courseId, string status, decimal? finalGrade, int? academicTerm, int? termYear);
    Task<List<StudyPlanCourse>> GetStudyPlanCoursesByCareerIdAsync(int careerId);
    Task<StudyPlanCourse?> GetStudyPlanCourseAsync(int studyPlanId, int courseId);
    Task<StudentProgress?> GetStudentProgressWithDetailAsync(int studentId, int courseId);
    Task<List<Requirement>> GetCoursePrerequisitesAsync(int courseId);
}
