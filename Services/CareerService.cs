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

    public async Task<List<RegistrationCareerResponse>> GetCareersByCampusForRegistrationAsync(int campusId)
    {
        var careers = await _careerRepository.GetActiveByCampusIdAsync(campusId);
        return careers.Select(c => new RegistrationCareerResponse
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            StudyPlans = c.StudyPlans.Select(sp => new RegistrationStudyPlanResponse
            {
                StudyPlanId = sp.StudyPlanId,
                Name = sp.Name,
                Code = sp.Code,
                ValidYear = sp.ValidYear
            }).ToList()
        }).ToList();
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
        var code = request.Code.Trim().ToUpperInvariant();
        var description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

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
            TotalCredits = request.TotalCredits,
            IsStatus = request.IsStatus,
            CreatedDate = DateTime.Now
        };

        var created = await _careerRepository.CreateAsync(career);
        return CareerMapper.ToResponse(created);
    }

    private static string NormalizeCareerCode(string officialResolution)
        => officialResolution.Trim().ToUpperInvariant();

    private static int GetCareerTotalCredits(string degreeLevel, int? bachelorCredits, int? diplomaCredits, int? degreeCredits)
    {
        var level = degreeLevel.Trim().ToUpperInvariant();
        return level switch
        {
            "BACHILLERATO" => bachelorCredits ?? 0,
            "DIPLOMADO"    => (bachelorCredits ?? 0) + (diplomaCredits ?? 0),
            _              => (bachelorCredits ?? 0) + (diplomaCredits ?? 0) + (degreeCredits ?? 0)
        };
    }

    private static string BuildAdminCareerDescription(
        string degreeLevel, int planYear, string school,
        int totalCredits, int? diplomaCredits, string officialResolution)
        => $"{degreeLevel.Trim()} | Plan {planYear} | {school.Trim()} | {totalCredits} créditos | Resolución: {officialResolution.Trim()}";

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
