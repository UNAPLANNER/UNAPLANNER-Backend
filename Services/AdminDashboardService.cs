using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IAdminDashboardRepository _dashboardRepository;

    public AdminDashboardService(IAdminDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<AdminDashboardResponse> GetDashboardAsync(int userId)
    {
        var dashboard = await _dashboardRepository.GetDashboardByAdminUserIdAsync(userId);
        if (dashboard == null)
            throw new KeyNotFoundException("No se encontro el perfil administrativo del usuario autenticado.");

        return dashboard;
    }
}
