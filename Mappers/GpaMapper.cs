using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Mappers;

public static class GpaMapper
{
    public static StudentGPAResponse ToResponse(int studentId, decimal gpa)
    {
        return new StudentGPAResponse
        {
            StudentId = studentId,
            Gpa = gpa,
            Message = "¡GPA calculado exitosamente!"
        };
    }
}