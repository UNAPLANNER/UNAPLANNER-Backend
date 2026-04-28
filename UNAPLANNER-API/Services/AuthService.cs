using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Repositories;
/*using UNAPLANNER_API.Helpers;*/



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

        if (user == null || user.Password != request.Password)
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

    public async Task<DeleteUserResponse> DeleteUserService(int id, string currentPassword)
{
    var user = await _userRepository.GetByIdAsync(id);
    if (user == null)
        throw new Exception("Usuario no encontrado");

    /*var isValid = PasswordHelper.VerifyPassword(currentPassword, user.Password);*/
    /*if (!isValid)
        throw new UnauthorizedAccessException("Contraseña incorrecta");

    await _userRepository.DeleteUser(user);*/

    return new DeleteUserResponse
    { 
        UserId = user.UserId, 
        Message = "Usuario eliminado correctamente" 
    };
}


    

}