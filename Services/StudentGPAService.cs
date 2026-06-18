using System.Security.Claims;
using System.Text;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Repositories;
using UNAPLANNER_API.Models.Entities;
using UNAPLANNER_API.Mappers;
using UNAPLANNER_API.Constants;

namespace UNAPLANNER_API.Services;

public class StudentGPAService : IStudentGPAService
{
    private readonly IStudentGPARepository _repository;

    public StudentGPAService(IStudentGPARepository repository)
    {
        _repository = repository;
    }

    /**Calculates the GPA (Grade Point Average) for a student based on approved courses.
        Only final grades from approved courses are considered.
        <param name="studentId">Student identifier.</param>
        A StudentGPAResponse containing the student ID and the calculated GPA.
        Returns 0 if the student has no approved courses with a final grade.**/
    public async Task<StudentGPAResponse> GetGpaAsync(int studentId)
    {
        var grades = await _repository.GetApprovedGradesAsync(studentId);

        if (grades == null || !grades.Any())
        throw new ArgumentException("¡Solicitud inválida ID incorrecto o parámetros inválidos!");
        var gpa = grades.Any()
            ? Math.Round(grades.Average(), 2)
            : 0;

        return GpaMapper.ToResponse(studentId, gpa);
    }
    
}