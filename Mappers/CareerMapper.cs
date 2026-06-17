using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Mappers;

public class CareerMapper
{
    public static CareerResponse ToResponse(Career career)
    {
        return new CareerResponse
        {
            Id = career.Id,
            Name = career.Name,
            Code = career.Code,
            Description = career.Description,
            TotalCredits = career.TotalCredits
        };
    }

    public static List<CareerResponse> ToResponseList(List<Career> careers)
    {
        return careers.Select(ToResponse).ToList();
    }
}
