using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public class EvaluationRepository : IEvaluationRepository
{
    private readonly AppDbContext _context;

    public EvaluationRepository(AppDbContext context)
    {
        _context = context;
    }
//Methods for handling assessments
    public async Task<StudentProgress?> GetStudentProgressAsync(int studentId, int courseId)
    {
        return await _context.StudentProgress
            .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.CourseId == courseId);
    }
//Method to get an evaluation by its ID, including the related student progress
    public async Task<Evaluation?> GetEvaluationByIdAsync(int evaluationId)
    {
        return await _context.Evaluations
            .Include(e => e.StudentProgress)
            .FirstOrDefaultAsync(e => e.Id == evaluationId);
    }
//Method to get evaluations for a specific student progress, ordered by date and creation time
    public async Task<List<Evaluation>> GetEvaluationsByStudentProgressAsync(int studentProgressId)
    {
        return await _context.Evaluations
            .Where(e => e.StudentProgressId == studentProgressId)
            .OrderBy(e => e.Date)
            .ThenBy(e => e.CreatedDate)
            .ToListAsync();
    }
//Method to create a new evaluation, returning the created evaluation with its related student progress
    public async Task<Evaluation?> CreateEvaluationAsync(Evaluation evaluation)
    {
        try
        {
            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync();

            return await _context.Evaluations
                .Include(e => e.StudentProgress)
                .FirstOrDefaultAsync(e => e.Id == evaluation.Id);
        }
        catch (Exception)
        {
            return null;
        }
    }
//Method to update an existing evaluation, returning the updated evaluation with its related student progress
    public async Task<Evaluation?> UpdateEvaluationAsync(Evaluation evaluation)
    {
        try
        {
            _context.Evaluations.Update(evaluation);
            await _context.SaveChangesAsync();

            return await _context.Evaluations
                .Include(e => e.StudentProgress)
                .FirstOrDefaultAsync(e => e.Id == evaluation.Id);
        }
        catch (Exception)
        {
            return null;
        }
    }
//Method to delete an evaluation by its ID, returning a boolean indicating success or failure
    public async Task<bool> DeleteEvaluationAsync(int evaluationId)
    {
        try
        {
            var evaluation = await _context.Evaluations.FindAsync(evaluationId);
            if (evaluation == null) return false;

            _context.Evaluations.Remove(evaluation);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
