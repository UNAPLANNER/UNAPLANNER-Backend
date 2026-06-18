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
}