using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Mappers;

public static class UserMapper
{
    public static UserResponse ToResponse(User user)
    {
        return new UserResponse
        {
            UserId = user.UserId,
            RoleId = user.RoleId,
            Email = user.Email,
            IsStatus = user.IsStatus,
            CreatedDate = user.CreatedDate
        };
    }

    public static User ToEntity(CreateUserRequest request)
    {
        return new User
        {
            RoleId = request.RoleId,
            Email = request.Email,
            Password = PasswordHelper.HashPassword(request.Password),
            IsStatus = true,
            CreatedDate = DateTime.Now
        };
    }
}
