using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Repositories;

namespace UNAPLANNER_API.Services;

public class CampusContactService : ICampusContactService
{
    private readonly ICampusContactRepository _campusContactRepository;

    public CampusContactService(ICampusContactRepository campusContactRepository)
    {
        _campusContactRepository = campusContactRepository;
    }

    public async Task<List<CampusContactResponse>> GetAllContactsAsync(int? campusId = null)
    {
        List<Models.Entities.CampusContact> contacts;

        if (campusId.HasValue && campusId > 0)
        {
            contacts = await _campusContactRepository.GetByCampusIdAsync(campusId.Value);
        }
        else
        {
            contacts = await _campusContactRepository.GetAllAsync();
        }

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
        var campusExists = await _campusContactRepository.CampusExistsAsync(request.CampusId);
        if (!campusExists)
            throw new InvalidOperationException($"El campus con ID {request.CampusId} no existe.");

        var entity = CampusContactMapper.ToEntity(request);
        var created = await _campusContactRepository.CreateAsync(entity);
        return CampusContactMapper.ToCampusContactResponse(created);
    }
}
