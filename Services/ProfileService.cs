using System.Text.RegularExpressions;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class ProfileService : IProfileService
{
    private static readonly Regex InstitutionalPhoneRegex = new(@"^2277-\d{4}$", RegexOptions.Compiled);
    private readonly IProfileRepository _profileRepository;

    public ProfileService(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<AdminProfileResponse?> GetAdminProfileAsync(int id)
    {
        var admin = await _profileRepository.GetAdminByUserIdAsync(id);
        return admin == null ? null : MapToResponse(admin);
    }

    public async Task<AdminProfileResponse?> UpdateAdminProfileAsync(int id, UpdateProfileRequest request)
    {
        var admin = await _profileRepository.GetAdminByUserIdAsync(id);
        if (admin == null) return null;

        admin.FullName = request.FullName.Trim();
        admin.Phone = request.Phone.Trim();
        admin.Department = request.Department.Trim();

        await _profileRepository.SaveChangesAsync();
        return MapToResponse(admin);
    }

    public async Task<PasswordChangeResult> ChangePasswordAsync(int id, ChangePasswordRequest request)
    {
        var admin = await _profileRepository.GetAdminByUserIdAsync(id);
        if (admin == null) return PasswordChangeResult.AdminNotFound;

        // The current password must match the stored hash before replacing it.
        if (!PasswordHelper.VerifyPassword(request.CurrentPassword, admin.User.Password))
        {
            return PasswordChangeResult.InvalidCurrentPassword;
        }

        admin.User.Password = PasswordHelper.HashPassword(request.NewPassword);
        await _profileRepository.SaveChangesAsync();
        return PasswordChangeResult.Success;
    }

    public static bool IsValidInstitutionalPhone(string phone)
    {
        return InstitutionalPhoneRegex.IsMatch(phone);
    }

    private static AdminProfileResponse MapToResponse(Admin admin)
    {
        return new AdminProfileResponse
        {
            UserId = admin.UserId,
            Email = admin.User.Email,
            Role = admin.User.Role.TypeRole,
            FullName = admin.FullName,
            Phone = admin.Phone,
            Department = admin.Department,
            CampusId = admin.CampusId
        };
    }
}
