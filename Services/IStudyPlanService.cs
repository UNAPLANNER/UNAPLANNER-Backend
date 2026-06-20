using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface IStudyPlanService
{
    Task<StudyPlanDetailResponse> GetStudyPlanDetailAsync(int studyPlanId);
    Task<StudyPlanDetailResponse> CreateStudyPlanAsync(CreateStudyPlanRequest request);
    Task<StudyPlanDetailResponse> CreateStudyPlanCourseAsync(int studyPlanId, CreateStudyPlanCourseRequest request);
    Task<StudyPlanDetailResponse> UpdateStudyPlanCourseAsync(int studyPlanId, int courseId, UpdateStudyPlanCourseRequest request);
}
