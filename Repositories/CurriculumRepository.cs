using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public class CurriculumRepository : ICurriculumRepository
{
    private readonly AppDbContext _context;

    public CurriculumRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetStudentByIdAsync(int studentId)
    {
        return await _context.Students
            .Include(s => s.StudyPlan).ThenInclude(sp => sp.Career)
            .FirstOrDefaultAsync(s => s.StudentId == studentId && s.IsStatus);
    }

    public async Task<Student?> GetStudentByUserIdAsync(int userId)
    {
        return await _context.Students
            .Include(s => s.StudyPlan).ThenInclude(sp => sp.Career)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsStatus);
    }

    public async Task<List<StudyPlanCourse>> GetStudyPlanCoursesAsync(int studyPlanId)
    {
        return await _context.StudyPlanCourses
            .Include(spc => spc.Course)
            .Where(spc => spc.StudyPlanId == studyPlanId && spc.IsStatus)
            .OrderBy(spc => spc.Levels)
            .ThenBy(spc => spc.Term)
            .ThenBy(spc => spc.Course.Name)
            .ToListAsync();
    }

    public async Task<List<StudentProgress>> GetStudentProgressAsync(int studentId)
    {
        return await _context.StudentProgress
            .Where(sp => sp.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<StudentProgress?> GetStudentProgressForCourseAsync(int studentId, int courseId)
    {
        return await _context.StudentProgress
            .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.CourseId == courseId);
    }

    public async Task<StudentProgress> UpsertStudentProgressAsync(
        int studentId, int courseId, string status,
        decimal? finalGrade, int? academicTerm, int? termYear)
    {
        var existing = await _context.StudentProgress
            .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.CourseId == courseId);

        if (existing == null)
        {
            existing = new StudentProgress
            {
                StudentId = studentId,
                CourseId = courseId
            };
            _context.StudentProgress.Add(existing);
        }

        existing.Status = status;
        existing.FinalGrade = finalGrade;
        existing.AcademicTerm = academicTerm;
        existing.TermYear = termYear;
        existing.LastUpdated = DateTime.Now;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<List<StudyPlanCourse>> GetStudyPlanCoursesByCareerIdAsync(int careerId)
    {
        return await _context.StudyPlanCourses
            .Include(spc => spc.Course)
            .Where(spc => spc.StudyPlan.CareerId == careerId && spc.IsStatus && spc.StudyPlan.IsStatus)
            .OrderBy(spc => spc.Levels)
            .ThenBy(spc => spc.Term)
            .ThenBy(spc => spc.Course.Name)
            .ToListAsync();
    }

    public async Task<StudyPlanCourse?> GetStudyPlanCourseAsync(int studyPlanId, int courseId)
    {
        return await _context.StudyPlanCourses
            .Include(spc => spc.Course)
            .FirstOrDefaultAsync(spc => spc.StudyPlanId == studyPlanId && spc.CourseId == courseId && spc.IsStatus);
    }

    public async Task<StudentProgress?> GetStudentProgressWithDetailAsync(int studentId, int courseId)
    {
        return await _context.StudentProgress
            .Include(sp => sp.StudentCourseDetail)
            .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.CourseId == courseId);
    }

    public async Task<List<Requirement>> GetCoursePrerequisitesAsync(int courseId)
    {
        return await _context.Requirements
            .Include(r => r.RequiredCourse)
            .Where(r => r.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<StudentCourseDetail> CreateCourseDetailAsync(int studentProgressId, string? professorName, string? classroom, string? schedule, string? syllabusUrl)
    {
        var detail = new StudentCourseDetail
        {
            StudentProgressId = studentProgressId,
            ProfessorName = professorName,
            Classroom = classroom,
            Schedule = schedule,
            SyllabusUrl = syllabusUrl
        };
        _context.StudentCourseDetails.Add(detail);
        await _context.SaveChangesAsync();
        return detail;
    }

    public async Task<StudentCourseDetail> UpsertCourseDetailAsync(int studentProgressId, string? professorName, string? classroom, string? schedule, string? syllabusUrl)
    {
        var existing = await _context.StudentCourseDetails
            .FirstOrDefaultAsync(d => d.StudentProgressId == studentProgressId);

        if (existing == null)
        {
            existing = new StudentCourseDetail { StudentProgressId = studentProgressId };
            _context.StudentCourseDetails.Add(existing);
        }

        existing.ProfessorName = professorName;
        existing.Classroom = classroom;
        existing.Schedule = schedule;
        existing.SyllabusUrl = syllabusUrl;

        await _context.SaveChangesAsync();
        return existing;
    }
}
