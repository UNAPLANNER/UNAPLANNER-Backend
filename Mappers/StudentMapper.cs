using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;

namespace UNAPLANNER_API.Mappers;

public static class StudentMapper
{
    // UPDATE,Request Entity
    public static void UpdateEntity(Student student, UpdateStudentProfileRequest request)
    {
        student.FullName = request.FullName;
        student.CareerId = request.CareerId;
        student.EnterYear = request.EnterYear;
    }

    // Entity Response
    public static UpdateStudentResponseDto ToResponse(Student student)
    {
        return new  UpdateStudentResponseDto
        {
            StudentId = student.StudentId,
            UserId = student.UserId,
            FullName = student.FullName,
            CareerId = student.CareerId,
            EnterYear = student.EnterYear
        };
    }
}