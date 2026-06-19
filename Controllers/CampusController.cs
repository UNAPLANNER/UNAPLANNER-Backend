using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;


[ApiController]
[Route("api/[controller]")]
public class CampusController : ControllerBase
{
    private readonly ICampusService _campusService;

    public CampusController(ICampusService service)
    {
        _campusService = service;
    }

    /// <summary>
    /// Retrieves the list of registered campuses.
    /// </summary>
    /// <returns>List of campuses with name, code, and status.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<CampusResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllCampus()
    {
        try
        {
            var campuses = await _campusService.GetAllCampus();

            if (campuses == null || !campuses.Any())
            {
                return NotFound(new
                {
                    message = "No campuses found"
                });
            }

            return Ok(campuses);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    message = "Error al obtener los campus",
                    error = ex.Message
                });
        }
    }

    /// <summary>
    /// Gets a campus by its unique identifier.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CampusResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCampusById(int id)
    {
        try
        {
            var campus = await _campusService.GetCampusByIdAsync(id);

            if (campus == null)
            {
                return NotFound(new
                {
                    message = $"No se encontró el campus con el ID {id}"
                   
                });
            }

            return Ok(campus);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    message = "Error retrieving campus",
                    error = ex.Message
                });
        }
    }

    /// <summary>
/// Creates a new campus with validation.
/// </summary>
/// <param name="request">Campus creation request DTO</param>
/// <returns>Created campus details if successful.</returns>
[HttpPost]
[ProducesResponseType(typeof(CampusResponseDto), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> CreateCampus([FromBody] CreateCampusRequest request)
{
    try
    {
        var result = await _campusService.CreateCampusAsync(request);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        // Correct: 201 Created with success message
        return CreatedAtAction(nameof(GetCampusById),
            new { id = result.Campus!.Id },
            new
            {
                message = "Se registro existosamente el campus",
                campus = result.Campus
            });
    }
    catch (Exception ex)
    {
        return StatusCode(StatusCodes.Status500InternalServerError,
            new { message = "Error creating campus", error = ex.Message });
    }
}

}