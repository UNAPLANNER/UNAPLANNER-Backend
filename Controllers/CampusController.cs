using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Route("api/campus")]
[ApiController]
public class CampusController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICareerService _careerService;

    public CampusController(AppDbContext context, ICareerService careerService)
    {
        _context = context;
        _careerService = careerService;
    }

    /// <summary>
    /// Lista todos los campus activos. Usado en el registro de estudiantes.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CampusResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCampuses()
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
