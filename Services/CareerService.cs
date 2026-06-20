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

    public CareerService(
        ICareerRepository careerRepository,
        ICurriculumRepository curriculumRepository,
        IProfileRepository profileRepository)
    {
        _careerRepository = careerRepository;
        _curriculumRepository = curriculumRepository;
        _profileRepository = profileRepository;
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
