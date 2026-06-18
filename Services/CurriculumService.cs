using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class CurriculumService : ICurriculumService
{
    private readonly ICurriculumRepository _curriculumRepository;

    public CurriculumService(ICurriculumRepository curriculumRepository)
    {
        _curriculumRepository = curriculumRepository;
    }

    public async Task<(bool Success, StudentCurriculumResponse? Curriculum, string? ErrorMessage)> GetStudentCurriculumAsync(int studentId)
    {
        try
        {
            var student = await _curriculumRepository.GetStudentByIdAsync(studentId);
            if (student == null)
                return (false, null, $"Estudiante con ID {studentId} no encontrado.");

            var studyPlanCourses = await _curriculumRepository.GetStudyPlanCoursesAsync(student.StudyPlanId);
            var progressList = await _curriculumRepository.GetStudentProgressAsync(studentId);

            var levels = CurriculumMapper.ToLevelCurriculumResponseList(studyPlanCourses, progressList);

            var career = student.StudyPlan.Career;

            var curriculum = new StudentCurriculumResponse
            {
                StudentId = student.StudentId,
                StudentName = student.FullName,
                CareerId = career.Id,
                CareerName = career.Name,
                CareerCode = career.Code,
                StudyPlanId = student.StudyPlanId,
                StudyPlanName = student.StudyPlan.Name,
                StudyPlanYear = student.StudyPlan.ValidYear,
                Levels = levels
            };

            return (true, curriculum, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al obtener la malla curricular: {ex.Message}");
        }
    }

    public async Task<(bool Success, List<StudentCourseProgressResponse>? Courses, string? ErrorMessage)> GetStudentCoursesWithProgressAsync(int studentId)
    {
        try
        {
            var student = await _curriculumRepository.GetStudentByIdAsync(studentId);
            if (student == null)
                return (false, null, $"Estudiante con ID {studentId} no encontrado.");

            var studyPlanCourses = await _curriculumRepository.GetStudyPlanCoursesAsync(student.StudyPlanId);
            var progressList = await _curriculumRepository.GetStudentProgressAsync(studentId);

            var result = CurriculumMapper.ToStudentCourseProgressList(studyPlanCourses, progressList);
            return (true, result, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al obtener los cursos: {ex.Message}");
        }
    }

    public async Task<(bool Success, StudentCourseProgressResponse? Course, string? ErrorMessage)> UpdateCourseStatusAsync(
        int studentId, int courseId, UpdateCourseStatusRequest request)
    {
        try
        {
            var student = await _curriculumRepository.GetStudentByIdAsync(studentId);
            if (student == null)
                return (false, null, $"Estudiante con ID {studentId} no encontrado.");

            var studyPlanCourses = await _curriculumRepository.GetStudyPlanCoursesAsync(student.StudyPlanId);
            var spc = studyPlanCourses.FirstOrDefault(c => c.CourseId == courseId);
            if (spc == null)
                return (false, null, $"El curso con ID {courseId} no pertenece al plan de estudios del estudiante.");

            var progress = await _curriculumRepository.UpsertStudentProgressAsync(
                studentId, courseId, request.Status,
                request.FinalGrade, request.Semester, request.Year);

            var response = CurriculumMapper.ToStudentCourseProgressResponse(spc, progress);
            return (true, response, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al actualizar el estado del curso: {ex.Message}");
        }
    }
}
