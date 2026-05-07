using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public class NotesRepository : INotesRepository
{
    private readonly AppDbContext _context;

    public NotesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetStudentByIdAsync(int studentId)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
    }

    public async Task<List<Note>> GetNotesByUserIdAsync(int userId, int? courseId = null)
    {
        var query = _context.Notes
            .OrderByDescending(n => n.CreatedAt)
            .Where(n => n.UserId == userId);

        if (courseId.HasValue)
        {
            query = query.Where(n => n.CourseId == courseId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Note?> CreateNoteAsync(Note note)
    {
        try
        {
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }
        catch (Exception)
        {
            return null;
        }
    }
}

