using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;
using Microsoft.AspNetCore.Authorization;
using UNAPLANNER_API.Constants;


namespace UNAPLANNER_API.Controllers;

[Authorize]
[Route("api/student")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly IStudentGPAService _serviceGPA;

    public StudentController(IStudentGPAService serviceGPA)
    {
        _serviceGPA = serviceGPA;
    }

    private bool IsCurrentStudent(int studentId)
    {
        var claim = User.FindFirstValue("StudentId");
        return int.TryParse(claim, out var id) && id == studentId;
    }

    [HttpGet("{id}/gpa")]
    [ProducesResponseType(typeof(StudentGPAResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGpa(int id)
    {
        if (!IsCurrentStudent(id)) return Forbid();
        try
        {
            var result = await _serviceGPA.GetGpaAsync(id);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "Error calculating GPA", error = ex.Message }
            );
        }
    }


}