using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface IAdminDashboardService
{
    Task<AdminDashboardResponse> GetDashboardAsync(int userId);
}
