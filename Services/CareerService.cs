using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class CareerService : ICareerService
{
    private readonly ICareerRepository _careerRepository;
    private readonly ICurriculumRepository _curriculumRepository;

    public CareerService(ICareerRepository careerRepository, ICurriculumRepository curriculumRepository)
    {
        _careerRepository = careerRepository;
        _curriculumRepository = curriculumRepository;
    }

    public async Task<List<CareerResponse>> GetAllCareersAsync()
    {
        var careers = await _careerRepository.GetAllAsync();
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
