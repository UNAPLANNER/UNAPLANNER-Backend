using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.DTOs.Responses;

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
    
}