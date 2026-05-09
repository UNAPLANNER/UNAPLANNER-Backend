using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface ICampusContactRepository
{
    Task<List<CampusContact>> GetAllAsync();
    Task<List<CampusContact>> GetByCampusIdAsync(int campusId);
    Task<CampusContact?> GetByIdAsync(int id);
    Task<CampusContact> AddAsync(CampusContact contact);
    Task<bool> SoftDeleteAsync(int id);
}
