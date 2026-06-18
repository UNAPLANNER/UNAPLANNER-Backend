using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;
public interface IStudentGPARepository
{
    Task<List<decimal>> GetApprovedGradesAsync(int studentId);
}