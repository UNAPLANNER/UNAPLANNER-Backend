using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface IStudyPlanRepository
{
    Task<StudyPlan?> GetDetailByIdAsync(int studyPlanId);
    Task<bool> CareerExistsAsync(int careerId);
    Task<bool> ExistsByCodeAsync(string code);
    Task<StudyPlan> CreateAsync(StudyPlan studyPlan);
}
