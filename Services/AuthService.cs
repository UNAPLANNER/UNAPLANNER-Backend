using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Repositories;
using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Constants;

namespace UNAPLANNER_API.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _userRepository;
    private readonly IConfiguration _config;
    private readonly IStudentRepository _studentRepository;

    public AuthService(IAuthRepository userRepository, IStudentRepository studentRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _config = config;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null) return null;

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

        if (!isPasswordValid)
            return null;

         var roleName = user.RoleId == RoleContants.Admin
            ? "Admin"
            : "Student";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.TypeRole),
            new Claim(ClaimTypes.Role, roleName) 
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var response = new AuthResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            Role = user.RoleId == RoleContants.Admin ? "Admin" : "Student",
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        };

        if (user.RoleId != RoleContants.Admin)
        {
            var student = await _studentRepository.GetStudentByUserIdAsync(user.UserId);
            if (student != null)
            {
                response.StudentId = student.StudentId;
                response.CareerId = student.StudyPlan?.Career?.Id ?? student.CareerId;
                response.StudyPlanId = student.StudyPlanId;
            }
        }

        return response;
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

        if (request.RoleId == RoleContants.Student)
        {
            var student = new Student
            {
                UserId = createdUser.UserId,
                FullName = request.FullName!,
                CareerId = request.CareerId!.Value,
                StudyPlanId = request.StudyPlanId!.Value,
                EnterYear = request.EnterYear!.Value
            };
            await _studentRepository.AddStudent(student);
        }
        return new UserResponse
        {
            UserId = createdUser.UserId,
            Email = createdUser.Email,
            RoleId = createdUser.RoleId,
            Role = createdUser.RoleId == RoleContants.Admin ? "Admin" : "Student",
            IsStatus = createdUser.IsStatus
        };
    }


}