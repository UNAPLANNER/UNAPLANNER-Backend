using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Services;

public interface ICampusContactService
{
    Task<List<CampusContactResponse>> GetAllContactsAsync(int? campusId = null);
    Task<List<CampusContactResponse>> GetContactsByCampusAsync(int campusId);
    Task<CampusContactResponse?> GetContactByIdAsync(int id);
}
