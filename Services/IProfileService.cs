using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface IProfileService
{
    Task<AdminProfileResponse?> GetAdminProfileAsync(int id);
    Task<AdminProfileResponse?> UpdateAdminProfileAsync(int id, UpdateProfileRequest request);
    Task<PasswordChangeResult> ChangePasswordAsync(int id, ChangePasswordRequest request);
}

public enum PasswordChangeResult
{
    Success,
    AdminNotFound,
    InvalidCurrentPassword
}
