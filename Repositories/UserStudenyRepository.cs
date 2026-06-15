using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public class UserStudentRepository : IUserStudentRepository
{
    private readonly AppDbContext _context;

    public UserStudentRepository(AppDbContext context)
    {
        _context = context;
    }

        public async Task<User?> GetByIdUserStudent(int id)
        {
            // Retrieves a user by ID including all related student data
           return await _context.Users
            .Include(u => u.Student)
            .ThenInclude(s => s.StudentProgress)
                .ThenInclude(sp => sp.Evaluations)
            .Include(u => u.Files)
            .Include(u => u.Calendars)
            .FirstOrDefaultAsync(u => u.UserId == id);
        }
        public async Task<bool> DeleteUserStudent(int id, string currentPassword)
        {
            // Retrieves the user with related data
            var user = await GetByIdUserStudent(id);
            if (user == null) return false;

            // Validates the current password before deletion
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(currentPassword, user.Password);
            if (!isPasswordValid) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

}
