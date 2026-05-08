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
}
