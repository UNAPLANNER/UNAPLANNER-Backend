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

    public async Task<Note?> GetNoteByIdAsync(int noteId)
    {
        return await _context.Notes
            .Include(n => n.Course)
            .FirstOrDefaultAsync(n => n.Id == noteId);
    }

    public async Task<List<Note>> GetNotesByUserIdAsync(int userId, int? courseId = null)
    {
        var query = _context.Notes
            .Include(n => n.Course)
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
            
            // Recargar la nota con el Course relacionado
            var createdNote = await _context.Notes
                .Include(n => n.Course)
                .FirstOrDefaultAsync(n => n.Id == note.Id);
            
            return createdNote;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Note?> UpdateNoteAsync(Note note)
    {
        try
        {
            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
            
            // Recargar la nota con el Course relacionado
            var updatedNote = await _context.Notes
                .Include(n => n.Course)
                .FirstOrDefaultAsync(n => n.Id == note.Id);
            
            return updatedNote;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> DeleteNoteAsync(int noteId)
    {
        try
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note == null)
            {
                return false;
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<Course>> GetStudentStudyPlanCoursesAsync(int studentId)
    {
        return await _context.StudentProgress
            .Where(sp => sp.StudentId == studentId && sp.Status == "EnCurso")
            .Include(sp => sp.Course)
            .Select(sp => sp.Course)
            .Where(c => c != null && c.IsStatus)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> IsStudentCourseEnCursoAsync(int studentId, int courseId)
    {
        return await _context.StudentProgress
            .AnyAsync(sp => sp.StudentId == studentId && sp.CourseId == courseId && sp.Status == "EnCurso");
    }

    public async Task<bool> IsStudentCourseEnCursoAsync(int studentId, int courseId)
    {
        return await _context.StudentProgress
            .AnyAsync(sp => sp.StudentId == studentId && sp.CourseId == courseId && sp.Status == "EnCurso");
    }
}

