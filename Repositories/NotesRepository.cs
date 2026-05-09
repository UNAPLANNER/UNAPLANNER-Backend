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

    public async Task<Student?> GetStudentByUserIdAsync(int userId)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId);
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

    public async Task<Note?> GetNoteByIdAsync(int noteId)
    {
        return await _context.Notes.FirstOrDefaultAsync(n => n.Id == noteId);
    }

    public async Task<Note> AddNoteAsync(Note note)
    {
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
        return note;
    }

    public Task UpdateNoteAsync(Note note)
    {
        _context.Notes.Update(note);
        return Task.CompletedTask;
    }

    public Task DeleteNoteAsync(Note note)
    {
        _context.Notes.Remove(note);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
