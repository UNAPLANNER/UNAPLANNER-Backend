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
    /**Checks if a career exists in the database by its ID.
    The ID of the career to verify
    Returns true if the career exists, otherwise false**/
    public async Task<bool> CareerExists(int careerId)
    {
        return await _context.Careers
            .AnyAsync(c => c.Id == careerId);
    }
    /**Updates the information of an existing student in the database.
    The student entity with updated data</param>
    Returns the updated student entity after saving changes or null if the update fails**/

    public async Task<Student?> UpdateStudentProfile(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();

        return await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == student.StudentId);
    }
    
    /**Retrieves a student from the database using the associated user ID.
    The ID of the user linked to the student
    Returns the student entity if found, otherwise null**/

    public async Task<Student?> GetStudentByUserId(int userId)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

}
