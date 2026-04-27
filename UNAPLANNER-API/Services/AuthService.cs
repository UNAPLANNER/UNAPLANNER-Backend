using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Repositories;
using UNAPLANNER_API.Models.Entities;


namespace UNAPLANNER_API.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _userRepository;
    private readonly IConfiguration _config;

    public AuthService(IAuthRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null) return null;

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

        if (!isPasswordValid)
            return null;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.TypeRole)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new AuthResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            Role = user.Role.TypeRole,
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        };
    }
    public async Task<UserResponse> RegisterUser(CreateUserRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("El correo ya está registrado.");

        var user = new User
        {
            RoleId = request.RoleId,
            Email = request.Email,
            Password = PasswordHelper.HashPassword(request.Password),
            IsStatus = true,
            CreatedDate = DateTime.Now
        };

        var createdUser = await _userRepository.AddUser(user);

        return new UserResponse
        {
            UserId = createdUser.UserId,
            RoleId = createdUser.RoleId,
            Email = createdUser.Email,
            IsStatus = createdUser.IsStatus,
            CreatedDate = createdUser.CreatedDate
        };
    }


}