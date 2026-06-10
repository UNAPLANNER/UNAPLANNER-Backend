using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public interface IStudentRepository
{
    Task<Student> AddStudent(Student student);

    /// <summary>
    /// Gets a student by their student ID
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <returns>Student if found, null otherwise</returns>
    Task<Student?> GetStudentByIdAsync(int studentId);
    Task<Student?> GetStudentByUserIdAsync(int userId);
}