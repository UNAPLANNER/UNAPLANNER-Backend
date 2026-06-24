using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.DTOs.Requests;

namespace UNAPLANNER_API.Mappers;
public static class CampusMapper
{
    public static CampusResponseDto ToDto(Campus campus)
    {
        return new CampusResponseDto
        {
            Id = campus.Id,
            Name = campus.Name,
            Code = campus.Code,
            IsStatus = campus.IsStatus
        };
    }

    public static List<CampusResponseDto> ToDtoList(List<Campus> campuses)
    {
        return campuses.Select(ToDto).ToList();
    }

    public static Campus ToEntity(CreateCampusRequest request)
    {
        return new Campus
        {
            Name = request.Name,
            Code = request.Code,
            IsStatus = true,
            CreatedDate = DateTime.Now
        };
    }
    
}