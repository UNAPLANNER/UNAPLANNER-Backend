using UNAPLANNER_API.DTOs.Requests;
using System.Linq;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Mappers;

public class CampusContactMapper
{
    public static CampusContactResponse ToCampusContactResponse(CampusContact campusContact)
    {
        return new CampusContactResponse
        {
            Id = campusContact.Id,
            CampusId = campusContact.CampusId,
            CampusName = campusContact.Campus?.Name ?? "N/A",
            DepartamentName = campusContact.DepartamentName,
            Phone = campusContact.Phone,
            Email = campusContact.Email,
            Description = campusContact.Description,
            IsStatus = campusContact.IsStatus,
            CreatedDate = campusContact.CreatedDate
        };
    }

    public static List<CampusContactResponse> ToCampusContactResponseList(List<CampusContact> campusContacts)
    {
        return campusContacts.Select(ToCampusContactResponse).ToList();
    }

    public static CampusContact ToEntity(CreateCampusContactRequest request)
    {
        return new CampusContact
        {
            CampusId = request.CampusId,
            DepartamentName = request.DepartamentName,
            Phone = request.Phone,
            Email = request.Email,
            Description = request.Description,
            IsStatus = true,
            CreatedDate = DateTime.Now
        };
    }

    /// <summary>
    /// Maps an UpdateCampusContactRequest to a CampusContact entity.
    /// Preserves the ID and creation date of the existing contact.
    /// </summary>
    /// <param name="request">The update request with new contact data</param>
    /// <param name="id">The ID of the contact being updated</param>
    /// <returns>A CampusContact entity with updated values</returns>
    public static CampusContact ToEntity(UpdateCampusContactRequest request, int id)
    {
        return new CampusContact
        {
            Id = id,
            CampusId = request.CampusId,
            DepartamentName = request.DepartamentName,
            Phone = request.Phone,
            Email = request.Email,
            Description = request.Description,
            IsStatus = true
        };
    }
}
