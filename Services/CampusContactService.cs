using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class CampusContactService : ICampusContactService
{
    private readonly ICampusContactRepository _campusContactRepository;

    public CampusContactService(ICampusContactRepository campusContactRepository)
    {
        _campusContactRepository = campusContactRepository;
    }

    public async Task<List<CampusContactResponse>> GetAllContactsAsync()
    {
        var contacts = await _campusContactRepository.GetAllAsync();
        return CampusContactMapper.ToCampusContactResponseList(contacts);
    }

    public async Task<List<CampusContactResponse>> GetContactsByCampusAsync(int campusId)
    {
        var contacts = await _campusContactRepository.GetByCampusIdAsync(campusId);
        return CampusContactMapper.ToCampusContactResponseList(contacts);
    }

    public async Task<CampusContactResponse?> GetContactByIdAsync(int id)
    {
        var contact = await _campusContactRepository.GetByIdAsync(id);
        if (contact == null) return null;
        
        return CampusContactMapper.ToCampusContactResponse(contact);
    }

    public async Task<CampusContactResponse> CreateContactAsync(CreateCampusContactRequest request)
    {
        var contact = new CampusContact
        {
            CampusId = request.CampusId,
            DepartamentName = request.DepartamentName.Trim(),
            Phone = request.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            IsStatus = true,
            CreatedDate = DateTime.Now
        };

        var created = await _campusContactRepository.AddAsync(contact);
        return CampusContactMapper.ToCampusContactResponse(created);
    }

    public async Task<bool> DeleteContactAsync(int id)
    {
        return await _campusContactRepository.SoftDeleteAsync(id);
    }
}
