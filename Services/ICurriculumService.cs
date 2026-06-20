using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface ICurriculumService
{
    Task<(bool Success, StudentCurriculumResponse? Curriculum, string? ErrorMessage)> GetStudentCurriculumAsync(int studentId);
    Task<(bool Success, List<StudentCourseProgressResponse>? Courses, string? ErrorMessage)> GetStudentCoursesWithProgressAsync(int studentId);
    Task<(bool Success, bool IsBadRequest, StudentCourseProgressResponse? Course, string? ErrorMessage)> UpdateCourseStatusAsync(int studentId, int courseId, UpdateCourseStatusRequest request);
    Task<(bool Success, CourseDetailResponse? Detail, string? ErrorMessage)> GetCourseDetailAsync(int studentId, int courseId);
    Task<(bool Success, bool IsConflict, bool IsBadRequest, CourseDetailResponse? Detail, string? ErrorMessage)> CreateEnrolledDetailAsync(int studentId, int courseId, EnrolledCourseDetailRequest request);
    Task<(bool Success, bool IsBadRequest, CourseDetailResponse? Detail, string? ErrorMessage)> UpdateEnrolledDetailAsync(int studentId, int courseId, EnrolledCourseDetailRequest request);
}
