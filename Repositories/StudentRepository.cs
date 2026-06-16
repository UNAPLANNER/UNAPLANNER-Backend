using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;
public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Student> AddStudent(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student?> GetStudentByIdAsync(int studentId)
    {
        return await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.StudentId == studentId && s.IsStatus);
    }

    public async Task<Student?> GetStudentByUserIdAsync(int userId)
    {
        return await _context.Students
            .Include(s => s.StudyPlan).ThenInclude(sp => sp.Career)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsStatus);
    }
}

