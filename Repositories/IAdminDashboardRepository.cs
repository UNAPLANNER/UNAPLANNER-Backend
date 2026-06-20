using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Repositories;

public interface IAdminDashboardRepository
{
    Task<AdminDashboardResponse?> GetDashboardByAdminUserIdAsync(int userId);
}
