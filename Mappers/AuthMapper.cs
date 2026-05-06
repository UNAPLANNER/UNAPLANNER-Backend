using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Mappers;

public static class AuthMapper
{
  
    public static AuthResponse ToResponse(User user, string token)
    {
        return new AuthResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            Token = token
        };
    }

    
}