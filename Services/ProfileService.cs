using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Repositories;
using BCrypt.Net;

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

        return new ProfileResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.Admin?.FullName ?? user.Student?.FullName,
            Phone = user.Admin?.Phone,
            Department = user.Admin?.Department,
            Role = user.Role.TypeRole
        };
    }

    public async Task<ProfileResponse?> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _repository.GetByIdAsync(userId);
        if (user == null) return null;

        if (user.Admin == null)
        {
            user.Admin = new Models.Entities.Admin { UserId = userId };
        }

        user.Admin.FullName = request.FullName ?? "";
        user.Admin.Phone = request.Phone;
        user.Admin.Department = request.Department;

        await _repository.UpdateAsync(user);
        var success = await _repository.SaveChangesAsync();

        if (!success) return null;

        return new ProfileResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.Admin?.FullName ?? user.Student?.FullName,
            Phone = user.Admin?.Phone,
            Department = user.Admin?.Department,
            Role = user.Role.TypeRole
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await _repository.GetByIdAsync(userId);
        if (user == null) return false;

        // Validar contraseña actual
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
        {
            return false;
        }

        // Hash de la nueva contraseña
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _repository.UpdateAsync(user);
        return await _repository.SaveChangesAsync();
    }
}
