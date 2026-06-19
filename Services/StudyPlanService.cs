using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class StudyPlanService : IStudyPlanService
{
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
}
