using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface IStudyPlanService
{
    Task<StudyPlanDetailResponse> GetStudyPlanDetailAsync(int studyPlanId);
}
