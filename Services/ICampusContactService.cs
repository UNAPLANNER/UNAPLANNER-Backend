using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.DTOs.Requests;

namespace UNAPLANNER_API.Services;

public interface ICampusContactService
{
    Task<List<CampusContactResponse>> GetAllContactsAsync();
    Task<List<CampusContactResponse>> GetContactsByCampusAsync(int campusId);
    Task<CampusContactResponse?> GetContactByIdAsync(int id);
    Task<CampusContactResponse> CreateContactAsync(CreateCampusContactRequest request);
    Task<bool> DeleteContactAsync(int id);
}
