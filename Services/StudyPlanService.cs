using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class StudyPlanService : IStudyPlanService
{
    private readonly IStudyPlanRepository _studyPlanRepository;

    public StudyPlanService(IStudyPlanRepository studyPlanRepository)
    {
        _studyPlanRepository = studyPlanRepository;
    }

    public async Task<StudyPlanDetailResponse> GetStudyPlanDetailAsync(int studyPlanId)
    {
        var studyPlan = await _studyPlanRepository.GetDetailByIdAsync(studyPlanId);
        if (studyPlan == null)
            throw new KeyNotFoundException("No se encontro el plan de estudios solicitado.");

        return StudyPlanMapper.ToDetailResponse(studyPlan);
    }
}
