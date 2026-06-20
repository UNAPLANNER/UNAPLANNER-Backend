using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class StudyPlanService : IStudyPlanService
{
    private static readonly HashSet<string> ValidElectiveTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Obligatorio",
        "OptativoDisciplinario",
        "OptativoLibre"
    };

    private readonly IStudyPlanRepository _studyPlanRepository;

    public StudyPlanService(IStudyPlanRepository studyPlanRepository)
    {
        _studyPlanRepository = studyPlanRepository;
    }

    public async Task<StudyPlanDetailResponse> GetStudyPlanDetailAsync(int studyPlanId)
    {
        var studyPlan = await _studyPlanRepository.GetDetailByIdAsync(studyPlanId);
        if (studyPlan == null)
            throw new KeyNotFoundException("No se encontro el plan de estudios solicitado.");

        return StudyPlanMapper.ToDetailResponse(studyPlan);
    }

    public async Task<StudyPlanDetailResponse> CreateStudyPlanAsync(CreateStudyPlanRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("El nombre del plan es obligatorio.", nameof(request.Name));

        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ArgumentException("El codigo del plan es obligatorio.", nameof(request.Code));

        var currentYear = DateTime.Now.Year;
        if (request.ValidYear is < 1900 || request.ValidYear > currentYear)
            throw new ArgumentException("El año de vigencia no puede ser mayor al actual.", nameof(request.ValidYear));

        var code = request.Code.Trim().ToUpperInvariant();

        if (!await _studyPlanRepository.CareerExistsAsync(request.CareerId))
            throw new KeyNotFoundException("No se encontro la carrera indicada.");

        if (await _studyPlanRepository.ExistsByCodeAsync(code))
            throw new InvalidOperationException("Ya existe un plan de estudios con ese codigo.");

        var studyPlan = new StudyPlan
        {
            CareerId = request.CareerId,
            Name = request.Name.Trim(),
            Code = code,
            ValidYear = request.ValidYear,
            IsStatus = request.IsStatus,
            CreatedDate = DateTime.Now
        };

        var created = await _studyPlanRepository.CreateAsync(studyPlan);
        return StudyPlanMapper.ToDetailResponse(created);
    }

    public async Task<StudyPlanDetailResponse> CreateStudyPlanCourseAsync(
        int studyPlanId,
        CreateStudyPlanCourseRequest request)
    {
        if (!await _studyPlanRepository.StudyPlanExistsAsync(studyPlanId))
            throw new KeyNotFoundException("No se encontro el plan de estudios solicitado.");

        var code = request.Code.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("El codigo del curso es obligatorio.", nameof(request.Code));

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("El nombre del curso es obligatorio.", nameof(request.Name));

        if (await _studyPlanRepository.CourseCodeExistsAsync(code))
            throw new InvalidOperationException("Ya existe un curso con ese codigo.");

        var electiveType = NormalizeElectiveType(request.ElectiveType, request.IsElective);
        var prerequisiteCourseIds = request.PrerequisiteCourseIds
            .Where(courseId => courseId > 0)
            .Distinct()
            .ToList();

        if (!await _studyPlanRepository.StudyPlanContainsCoursesAsync(studyPlanId, prerequisiteCourseIds))
            throw new ArgumentException("Los requisitos deben pertenecer al mismo plan de estudios.", nameof(request.PrerequisiteCourseIds));

        var course = new Course
        {
            Code = code,
            Name = request.Name.Trim(),
            Credits = request.Credits,
            TheoryHours = request.TheoryHours,
            PracticeHours = request.PracticeHours,
            LabHours = request.LabHours,
            IsStatus = request.IsStatus,
            CreatedDate = DateTime.Now
        };

        var studyPlanCourse = new StudyPlanCourse
        {
            StudyPlanId = studyPlanId,
            Levels = request.Level,
            Term = request.Term,
            IsElective = request.IsElective,
            ElectiveType = electiveType,
            IsStatus = request.IsStatus,
            CreatedDate = DateTime.Now
        };

        var requirements = prerequisiteCourseIds
            .Select(requiredCourseId => new Requirement
            {
                RequiredCourseId = requiredCourseId,
                RequirementType = "Prerequisite",
                CreatedDate = DateTime.Now
            })
            .ToList();

        var updatedStudyPlan = await _studyPlanRepository.CreateCourseAsync(
            studyPlanId,
            course,
            studyPlanCourse,
            requirements);

        return StudyPlanMapper.ToDetailResponse(updatedStudyPlan);
    }

    public async Task<StudyPlanDetailResponse> UpdateStudyPlanCourseAsync(
        int studyPlanId,
        int courseId,
        UpdateStudyPlanCourseRequest request)
    {
        if (!await _studyPlanRepository.StudyPlanExistsAsync(studyPlanId))
            throw new KeyNotFoundException("No se encontro el plan de estudios solicitado.");

        var planCourse = await _studyPlanRepository.GetStudyPlanCourseForUpdateAsync(studyPlanId, courseId);
        if (planCourse == null)
            throw new KeyNotFoundException("No se encontro el curso en el plan de estudios.");

        var code = request.Code.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("El codigo del curso es obligatorio.", nameof(request.Code));

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("El nombre del curso es obligatorio.", nameof(request.Name));

        if (await _studyPlanRepository.CourseCodeExistsExcludingCourseAsync(code, courseId))
            throw new InvalidOperationException("Ya existe un curso con ese codigo.");

        var electiveType = NormalizeElectiveType(request.ElectiveType, request.IsElective);
        var prerequisiteCourseIds = request.PrerequisiteCourseIds
            .Where(requiredCourseId => requiredCourseId > 0 && requiredCourseId != courseId)
            .Distinct()
            .ToList();

        if (!await _studyPlanRepository.StudyPlanContainsCoursesAsync(studyPlanId, prerequisiteCourseIds))
            throw new ArgumentException("Los requisitos deben pertenecer al mismo plan de estudios.", nameof(request.PrerequisiteCourseIds));

        planCourse.Course.Code = code;
        planCourse.Course.Name = request.Name.Trim();
        planCourse.Course.Credits = request.Credits;
        planCourse.Course.TheoryHours = request.TheoryHours;
        planCourse.Course.PracticeHours = request.PracticeHours;
        planCourse.Course.LabHours = request.LabHours;
        planCourse.Course.IsStatus = request.IsStatus;

        planCourse.Levels = request.Level;
        planCourse.Term = request.Term;
        planCourse.IsElective = request.IsElective;
        planCourse.ElectiveType = electiveType;
        planCourse.IsStatus = request.IsStatus;

        var requirements = prerequisiteCourseIds
            .Select(requiredCourseId => new Requirement
            {
                CourseId = courseId,
                RequiredCourseId = requiredCourseId,
                RequirementType = "Prerequisite",
                CreatedDate = DateTime.Now
            })
            .ToList();

        var updatedStudyPlan = await _studyPlanRepository.UpdateCourseAsync(
            studyPlanId,
            courseId,
            planCourse.Course,
            planCourse,
            requirements);

        return StudyPlanMapper.ToDetailResponse(updatedStudyPlan);
    }

    private static string NormalizeElectiveType(string electiveType, bool isElective)
    {
        var normalizedElectiveType = string.IsNullOrWhiteSpace(electiveType)
            ? "Obligatorio"
            : electiveType.Trim();

        if (!ValidElectiveTypes.Contains(normalizedElectiveType))
            throw new ArgumentException("El tipo de curso no es valido.", nameof(electiveType));

        if (!isElective && !normalizedElectiveType.Equals("Obligatorio", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Un curso obligatorio debe usar el tipo Obligatorio.", nameof(electiveType));

        if (isElective && normalizedElectiveType.Equals("Obligatorio", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Un curso optativo debe usar un tipo optativo.", nameof(electiveType));

        return normalizedElectiveType;
    }
}
