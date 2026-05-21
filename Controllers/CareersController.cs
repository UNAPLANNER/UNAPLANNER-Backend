using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Responses;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/careers")]
public class CareersController : ControllerBase
{
    private readonly ICareerService _careerService;

    public CareersController(ICareerService careerService)
    {
        _careerService = careerService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CareerResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CareerResponse>>> GetAll()
    {
        var careers = await _careerService.GetAllAsync();
        return Ok(careers);
    }
}
