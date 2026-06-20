using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;
using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;

namespace UNAPLANNER_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampusController : ControllerBase
{
    private readonly ICampusService _campusService;
    private readonly AppDbContext _context;
    private readonly ICareerService _careerService;

    public CampusController(ICampusService service, AppDbContext context, ICareerService careerService)
    {
        _campusService = service;
        _context = context;
        _careerService = careerService;
    }

    /// <summary>
    /// Retrieves the list of registered campuses (active and inactive). Used in the admin screen.
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
    [HttpGet("{id:int}")]
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

            return CreatedAtAction(nameof(GetCampusById),
                new { id = result.Campus!.Id },
                new
                {
                    message = "Se registró exitosamente el campus",
                    campus = result.Campus
                });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error creating campus", error = ex.Message });
        }
    }

    /// <summary>
    /// Lista solo los campus activos. Usado en el registro de estudiantes.
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(List<CampusResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveCampuses()
    {
        var campuses = await _context.Campuses
            .AsNoTracking()
            .Where(c => c.IsStatus)
            .OrderBy(c => c.Name)
            .Select(c => new CampusResponse
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code
            })
            .ToListAsync();

        return Ok(campuses);
    }

    /// <summary>
    /// Retorna las carreras activas de un campus con sus planes de estudio.
    /// Si el campus no tiene carreras, retorna 404 con mensaje.
    /// </summary>
    [HttpGet("{campusId}/careers")]
    [ProducesResponseType(typeof(List<RegistrationCareerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCareersByCampus(int campusId)
    {
        try
        {
            var campusExists = await _context.Campuses
                .AnyAsync(c => c.Id == campusId && c.IsStatus);

            if (!campusExists)
                return NotFound(new { message = "Campus no encontrado." });

            var careers = await _careerService.GetCareersByCampusForRegistrationAsync(campusId);

            if (careers.Count == 0)
                return NotFound(new { message = "Este campus no tiene carreras disponibles." });

            return Ok(careers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener las carreras.", detail = ex.Message });
        }
    }
}
