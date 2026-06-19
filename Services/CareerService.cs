using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class CareerService : ICareerService
{
    private readonly ICareerRepository _careerRepository;
    private readonly ICurriculumRepository _curriculumRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IStudyPlanService _studyPlanService;

    public CareerService(
        ICareerRepository careerRepository,
        ICurriculumRepository curriculumRepository,
        IProfileRepository profileRepository,
        IStudyPlanService studyPlanService)
    {
        _careerRepository = careerRepository;
        _curriculumRepository = curriculumRepository;
        _profileRepository = profileRepository;
        _studyPlanService = studyPlanService;
    }

    public async Task<List<CareerResponse>> GetAllCareersAsync()
    {
        var careers = await _careerRepository.GetAllAsync();
        return CareerMapper.ToResponseList(careers);
    }

    public async Task<List<CareerResponse>> GetCareersByAdminUserIdAsync(int userId)
    {
        var admin = await _profileRepository.GetAdminByUserIdAsync(userId);
        if (admin == null)
            throw new KeyNotFoundException("No se encontro el perfil administrativo del usuario autenticado.");

        var careers = await _careerRepository.GetByCampusIdAsync(admin.CampusId);
        return CareerMapper.ToResponseList(careers);
    }

    public async Task<CareerResponse> UpdateCareerForAdminAsync(int userId, int careerId, UpdateCareerRequest request)
    {
        var admin = await _profileRepository.GetAdminByUserIdAsync(userId);
        if (admin == null)
            throw new KeyNotFoundException("No se encontro el perfil administrativo del usuario autenticado.");

        var career = await _careerRepository.GetEditableByIdAsync(careerId);
        if (career == null || career.CampusId != admin.CampusId)
            throw new KeyNotFoundException("No se encontro la carrera solicitada.");

        var normalizedCode = NormalizeCareerCode(request.OfficialResolution);
        if (await _careerRepository.ExistsByCodeExcludingIdAsync(careerId, normalizedCode))
            throw new InvalidOperationException("Ya existe una carrera con ese codigo.");

        var totalCredits = GetCareerTotalCredits(
            request.DegreeLevel,
            request.BachelorCredits,
            request.DiplomaCredits,
            request.DegreeCredits);

        career.Name = request.Name.Trim();
        career.Code = normalizedCode;
        career.Description = BuildAdminCareerDescription(
            request.DegreeLevel,
            request.PlanYear,
            request.School,
            totalCredits,
            request.DiplomaCredits,
            request.OfficialResolution);
        career.TotalCredits = totalCredits;
        career.IsStatus = request.IsStatus;

        var updated = await _careerRepository.UpdateAsync(career);
        return CareerMapper.ToResponse(updated);
    }

    public async Task<CareerResponse> CreateCareerForAdminAsync(int userId, CreateCareerRequest request)
    {
        var admin = await _profileRepository.GetAdminByUserIdAsync(userId);
        if (admin == null)
            throw new KeyNotFoundException("No se encontro el perfil administrativo del usuario autenticado.");

        var name = request.Name.Trim();
        var code = NormalizeCareerCode(request.OfficialResolution);
        var totalCredits = GetCareerTotalCredits(
            request.DegreeLevel,
            request.BachelorCredits,
            request.DiplomaCredits,
            request.DegreeCredits);
        var description = BuildAdminCareerDescription(
            request.DegreeLevel,
            request.PlanYear,
            request.School,
            totalCredits,
            request.DiplomaCredits,
            request.OfficialResolution);

        if (await _careerRepository.ExistsByNameAsync(admin.CampusId, name))
            throw new InvalidOperationException("Ya existe una carrera con ese nombre en este campus.");

        if (await _careerRepository.ExistsByCodeAsync(admin.CampusId, code))
            throw new InvalidOperationException("Ya existe una carrera con ese codigo en este campus.");

        var career = new Career
        {
            CampusId = admin.CampusId,
            Name = name,
            Code = code,
            Description = description,
            TotalCredits = totalCredits,
            IsStatus = request.IsStatus,
            CreatedDate = DateTime.Now
        };

        var created = await _careerRepository.CreateAsync(career);
        await _studyPlanService.CreateStudyPlanAsync(new CreateStudyPlanRequest
        {
            CareerId = created.Id,
            Name = BuildStudyPlanName(request.DegreeLevel, name),
            Code = BuildStudyPlanCode(request.DegreeLevel, code, request.PlanYear),
            ValidYear = request.PlanYear,
            IsStatus = request.IsStatus
        });

        var createdWithStudyPlan = await _careerRepository.GetEditableByIdAsync(created.Id);
        return CareerMapper.ToResponse(createdWithStudyPlan ?? created);
    }

    private static string NormalizeCareerCode(string officialResolution)
    {
        return officialResolution.Trim().ToUpperInvariant();
    }

    private static string BuildStudyPlanName(string degreeLevel, string careerName)
    {
        return $"{degreeLevel.Trim()} en {careerName.Trim()}";
    }

    private static string BuildStudyPlanCode(string degreeLevel, string careerCode, int planYear)
    {
        var degreePrefix = degreeLevel.Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word[0])
            .Aggregate(string.Empty, (current, letter) => current + letter)
            .ToUpperInvariant();

        return $"{degreePrefix}-{careerCode.Trim().ToUpperInvariant()}-{planYear}";
    }

    private static int GetCareerTotalCredits(
        string degreeLevel,
        int? bachelorCredits,
        int? diplomaCredits,
        int? degreeCredits)
    {
        if (RequiresSpecificDegreeCredits(degreeLevel))
        {
            return degreeCredits
                ?? throw new InvalidOperationException($"Los creditos de {degreeLevel.Trim().ToLowerInvariant()} son obligatorios.");
        }

        if (!diplomaCredits.HasValue)
            throw new InvalidOperationException("Los creditos de diplomado son obligatorios.");

        return bachelorCredits
            ?? throw new InvalidOperationException("Los creditos de bachillerato son obligatorios.");
    }

    private static string BuildAdminCareerDescription(
        string degreeLevel,
        int planYear,
        string school,
        int totalCredits,
        int? diplomaCredits,
        string officialResolution)
    {
        var normalizedDegree = degreeLevel.Trim();
        var creditsDescription = RequiresSpecificDegreeCredits(normalizedDegree)
            ? $"Creditos {normalizedDegree.ToLowerInvariant()}: {totalCredits}"
            : $"Creditos diplomado: {diplomaCredits}";

        return string.Join(" | ", new[]
        {
            $"Grado: {normalizedDegree}",
            $"Anio del plan: {planYear}",
            $"Escuela: {school.Trim()}",
            creditsDescription,
            $"Resolucion oficial: {officialResolution.Trim().ToUpperInvariant()}"
        });
    }

    private static bool RequiresSpecificDegreeCredits(string degreeLevel)
    {
        var normalizedDegree = degreeLevel.Trim();
        return normalizedDegree.Equals("Licenciatura", StringComparison.OrdinalIgnoreCase)
            || normalizedDegree.Equals("Maestria", StringComparison.OrdinalIgnoreCase)
            || normalizedDegree.Equals("Maestría", StringComparison.OrdinalIgnoreCase)
            || normalizedDegree.Equals("Doctorado", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<List<LevelCurriculumResponse>> GetCurriculumByCareerIdAsync(int careerId, int? userId = null)
    {
        var career = await _careerRepository.GetByIdAsync(careerId);
        if (career == null)
            throw new KeyNotFoundException($"La carrera con ID {careerId} no existe.");

        List<StudyPlanCourse> studyPlanCourses;
        List<StudentProgress> progressList = new();

        if (userId.HasValue)
        {
            var student = await _curriculumRepository.GetStudentByUserIdAsync(userId.Value);
            if (student != null)
            {
                studyPlanCourses = await _curriculumRepository.GetStudyPlanCoursesAsync(student.StudyPlanId);
                progressList = await _curriculumRepository.GetStudentProgressAsync(student.StudentId);
                return CurriculumMapper.ToLevelCurriculumResponseList(studyPlanCourses, progressList);
            }
        }

        studyPlanCourses = await _curriculumRepository.GetStudyPlanCoursesByCareerIdAsync(careerId);
        return CurriculumMapper.ToLevelCurriculumResponseList(studyPlanCourses, progressList);
    }
}
