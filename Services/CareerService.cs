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
