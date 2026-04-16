using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}