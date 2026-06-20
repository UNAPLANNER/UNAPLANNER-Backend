using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Repositories;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Services;

public class CurriculumService : ICurriculumService
{
    private readonly ICurriculumRepository _curriculumRepository;

    public CurriculumService(ICurriculumRepository curriculumRepository)
    {
        _curriculumRepository = curriculumRepository;
    }

    // Tries StudentId first, falls back to UserId so the app can send either without breaking.
    private async Task<Student?> ResolveStudentAsync(int id)
        => await _curriculumRepository.GetStudentByIdAsync(id)
           ?? await _curriculumRepository.GetStudentByUserIdAsync(id);

    public async Task<(bool Success, StudentCurriculumResponse? Curriculum, string? ErrorMessage)> GetStudentCurriculumAsync(int studentId)
    {
        try
        {
            var student = await ResolveStudentAsync(studentId);
            if (student == null)
                return (false, null, $"Estudiante con ID {studentId} no encontrado.");

            var sid = student.StudentId;
            var studyPlanCourses = await _curriculumRepository.GetStudyPlanCoursesAsync(student.StudyPlanId);
            var progressList = await _curriculumRepository.GetStudentProgressAsync(sid);

            var levels = CurriculumMapper.ToLevelCurriculumResponseList(studyPlanCourses, progressList);
            var career = student.StudyPlan.Career;

            var curriculum = new StudentCurriculumResponse
            {
                StudentId = sid,
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
            var student = await ResolveStudentAsync(studentId);
            if (student == null)
                return (false, null, $"Estudiante con ID {studentId} no encontrado.");

            var sid = student.StudentId;
            var studyPlanCourses = await _curriculumRepository.GetStudyPlanCoursesAsync(student.StudyPlanId);
            var progressList = await _curriculumRepository.GetStudentProgressAsync(sid);

            var result = CurriculumMapper.ToStudentCourseProgressList(studyPlanCourses, progressList);
            return (true, result, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al obtener los cursos: {ex.Message}");
        }
    }

    public async Task<(bool Success, bool IsBadRequest, StudentCourseProgressResponse? Course, string? ErrorMessage)> UpdateCourseStatusAsync(
        int studentId, int courseId, UpdateCourseStatusRequest request)
    {
        try
        {
            var student = await ResolveStudentAsync(studentId);
            if (student == null)
                return (false, false, null, $"Estudiante con ID {studentId} no encontrado.");

            var sid = student.StudentId;
            var studyPlanCourses = await _curriculumRepository.GetStudyPlanCoursesAsync(student.StudyPlanId);
            var spc = studyPlanCourses.FirstOrDefault(c => c.CourseId == courseId);
            if (spc == null)
                return (false, false, null, $"El curso con ID {courseId} no pertenece al plan de estudios del estudiante.");

            var currentProgress = await _curriculumRepository.GetStudentProgressForCourseAsync(sid, courseId);
            var currentStatus = currentProgress?.Status ?? "Pendiente";

            if (currentStatus == "Aprobado" && request.Status != "Aprobado")
                return (false, true, null, $"El curso con ID {courseId} ya está Aprobado y no puede cambiar de estado.");

            bool gradeApplies = request.Status == "Aprobado" || request.Status == "Reprobado";
            bool isPending    = request.Status == "Pendiente";
            int? safeYear     = (request.Year is > 0) ? request.Year : null;

            var progress = await _curriculumRepository.UpsertStudentProgressAsync(
                sid, courseId, request.Status,
                gradeApplies ? request.FinalGrade : null,
                isPending ? null : request.Semester,
                isPending ? null : safeYear);

            var response = CurriculumMapper.ToStudentCourseProgressResponse(spc, progress);
            return (true, false, response, null);
        }
        catch (Exception ex)
        {
            return (false, false, null, $"Error al actualizar el estado del curso: {ex.Message}");
        }
    }

    public async Task<(bool Success, bool IsConflict, bool IsBadRequest, CourseDetailResponse? Detail, string? ErrorMessage)> CreateEnrolledDetailAsync(
        int studentId, int courseId, EnrolledCourseDetailRequest request)
    {
        try
        {
            var student = await ResolveStudentAsync(studentId);
            if (student == null)
                return (false, false, false, null, $"Estudiante con ID {studentId} no encontrado.");

            var sid = student.StudentId;
            var spc = await _curriculumRepository.GetStudyPlanCourseAsync(student.StudyPlanId, courseId);
            if (spc == null)
                return (false, false, false, null, $"El curso con ID {courseId} no pertenece al plan de estudios del estudiante.");

            var progress = await _curriculumRepository.GetStudentProgressWithDetailAsync(sid, courseId);
            if (progress?.Status != "EnCurso")
                return (false, false, true, null, "Solo se pueden registrar detalles para cursos con estado 'EnCurso'.");

            if (progress.StudentCourseDetail != null)
                return (false, true, false, null, "Ya existe un detalle de matrícula para este curso. Use PUT para actualizarlo.");

            var detail = await _curriculumRepository.CreateCourseDetailAsync(
                progress.StudentProgressId, request.ProfessorName, request.Classroom, request.Schedule, request.SyllabusUrl);

            progress.StudentCourseDetail = detail;

            var prerequisites = await _curriculumRepository.GetCoursePrerequisitesAsync(courseId);
            var prereqProgressDict = await BuildPrereqProgressDict(sid, prerequisites);

            var detailResponse = CurriculumMapper.ToCourseDetailResponse(spc, progress, prerequisites, prereqProgressDict);
            return (true, false, false, detailResponse, null);
        }
        catch (Exception ex)
        {
            return (false, false, false, null, $"Error al crear el detalle del curso: {ex.Message}");
        }
    }

    public async Task<(bool Success, bool IsBadRequest, CourseDetailResponse? Detail, string? ErrorMessage)> UpdateEnrolledDetailAsync(
        int studentId, int courseId, EnrolledCourseDetailRequest request)
    {
        try
        {
            var student = await ResolveStudentAsync(studentId);
            if (student == null)
                return (false, false, null, $"Estudiante con ID {studentId} no encontrado.");

            var sid = student.StudentId;
            var spc = await _curriculumRepository.GetStudyPlanCourseAsync(student.StudyPlanId, courseId);
            if (spc == null)
                return (false, false, null, $"El curso con ID {courseId} no pertenece al plan de estudios del estudiante.");

            var progress = await _curriculumRepository.GetStudentProgressWithDetailAsync(sid, courseId);
            if (progress?.Status != "EnCurso")
                return (false, true, null, "Solo se pueden actualizar detalles para cursos con estado 'EnCurso'.");

            var detail = await _curriculumRepository.UpsertCourseDetailAsync(
                progress.StudentProgressId, request.ProfessorName, request.Classroom, request.Schedule, request.SyllabusUrl);

            progress.StudentCourseDetail = detail;

            var prerequisites = await _curriculumRepository.GetCoursePrerequisitesAsync(courseId);
            var prereqProgressDict = await BuildPrereqProgressDict(sid, prerequisites);

            var detailResponse = CurriculumMapper.ToCourseDetailResponse(spc, progress, prerequisites, prereqProgressDict);
            return (true, false, detailResponse, null);
        }
        catch (Exception ex)
        {
            return (false, false, null, $"Error al actualizar el detalle del curso: {ex.Message}");
        }
    }

    public async Task<(bool Success, CourseDetailResponse? Detail, string? ErrorMessage)> GetCourseDetailAsync(int studentId, int courseId)
    {
        try
        {
            var student = await ResolveStudentAsync(studentId);
            if (student == null)
                return (false, null, $"Estudiante con ID {studentId} no encontrado.");

            var sid = student.StudentId;
            var spc = await _curriculumRepository.GetStudyPlanCourseAsync(student.StudyPlanId, courseId);
            if (spc == null)
                return (false, null, $"El curso con ID {courseId} no pertenece al plan de estudios del estudiante.");

            var progress = await _curriculumRepository.GetStudentProgressWithDetailAsync(sid, courseId);
            var prerequisites = await _curriculumRepository.GetCoursePrerequisitesAsync(courseId);
            var prereqProgressDict = await BuildPrereqProgressDict(sid, prerequisites);

            var detail = CurriculumMapper.ToCourseDetailResponse(spc, progress, prerequisites, prereqProgressDict);
            return (true, detail, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al obtener el detalle del curso: {ex.Message}");
        }
    }

    private async Task<Dictionary<int, StudentProgress>> BuildPrereqProgressDict(int studentId, List<Requirement> prerequisites)
    {
        if (prerequisites.Count == 0)
            return new Dictionary<int, StudentProgress>();

        var allProgress = await _curriculumRepository.GetStudentProgressAsync(studentId);
        var prereqIds = prerequisites.Select(r => r.RequiredCourseId).ToHashSet();
        return allProgress.Where(p => prereqIds.Contains(p.CourseId)).ToDictionary(p => p.CourseId);
    }
}
