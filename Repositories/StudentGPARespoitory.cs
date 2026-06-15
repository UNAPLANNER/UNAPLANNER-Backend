using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;


public class StudentGPARepository : IStudentGPARepository
{
    private readonly AppDbContext _context;

    public StudentGPARepository(AppDbContext context)
    {
        _context = context;
    }
    /**
     Retrieves all approved final grades for a specific student.
     Only courses with status "Aprobado" and a valid FinalGrade are included.
     <param name="studentId">Student identifier.</param>
     A list of final grades from approved courses.
    Courses without a final grade are excluded from the result**/
    public async Task<List<decimal>> GetApprovedGradesAsync(int studentId)
    {
        return await _context.StudentProgress
            .Where(x =>
                x.StudentId == studentId &&
                x.Status == "Aprobado" &&
                x.FinalGrade.HasValue)
            .Select(x => x.FinalGrade ?? 0)
            .ToListAsync();
    }
}