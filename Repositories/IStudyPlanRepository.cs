using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface IStudyPlanRepository
{
    Task<StudyPlan?> GetDetailByIdAsync(int studyPlanId);
}
