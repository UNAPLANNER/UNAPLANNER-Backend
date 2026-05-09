using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _repository;

    public ProfileService(IProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProfileResponse?> GetProfileAsync(int userId)
    {
        var user = await _repository.GetByIdAsync(userId);
        if (user == null) return null;

        var admin = user.Admin ?? await _repository.GetAdminByUserIdAsync(userId);

        return MapProfile(user, admin);
    }

    public async Task<ProfileResponse?> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _repository.GetByIdAsync(userId);
        if (user == null) return null;

        var admin = user.Admin ?? await _repository.GetAdminByUserIdAsync(userId);

        if (admin == null)
        {
            admin = new Admin { UserId = userId };
            await _repository.AddAdminAsync(admin);
        }

        admin.FullName = request.FullName ?? admin.FullName;
        admin.Phone = request.Phone ?? admin.Phone;
        admin.Department = request.Department ?? admin.Department;

        await _repository.SaveChangesAsync();

        return MapProfile(user, admin);
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await _repository.GetByIdAsync(userId);
        if (user == null) return false;

        var currentPassword = request.CurrentPassword ?? request.OldPassword;
        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return false;
        }

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
        {
            return false;
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _repository.UpdateAsync(user);
        return await _repository.SaveChangesAsync();
    }

    private static ProfileResponse MapProfile(User user, Admin? admin)
    {
        return new ProfileResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = admin?.FullName ?? user.Student?.FullName,
            Phone = admin?.Phone,
            Department = admin?.Department,
            Role = user.Role.TypeRole
        };
    }
}
