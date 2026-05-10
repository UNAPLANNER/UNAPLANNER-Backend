using System.Text;
using Microsoft.IdentityModel.Tokens;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Repositories;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Services;

    public class UserStudentService : IUserStudentService
    {
        private readonly IUserStudentRepository _userStudentRepository;

        public UserStudentService(IUserStudentRepository userStudentRepository)
        {
            _userStudentRepository = userStudentRepository;
        }

        public async Task<bool> DeleteUserStudent(int id, string currentPassword)
        {
            var user = await _userStudentRepository.GetByIdUserStudent(id);
            if (user == null) return false;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(currentPassword, user.Password);
            if (!isPasswordValid) return false;

            return await _userStudentRepository.DeleteUserStudent(id, currentPassword);
        }
    }

