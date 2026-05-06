using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Route("api/campus-contacts")]
[ApiController]
public class CampusContactsController : ControllerBase
{
    private readonly ICampusContactService _campusContactService;

    public CampusContactsController(ICampusContactService campusContactService)
    {
        _campusContactService = campusContactService;
    }

    /// <summary>
    /// Obtiene todos los contactos de campus disponibles
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllContacts()
    {
        try
        {
            var contacts = await _campusContactService.GetAllContactsAsync();
            return Ok(contacts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al obtener los contactos", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene los contactos de un campus específico
    /// </summary>
    [HttpGet("campus/{campusId}")]
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetContactsByCampus(int campusId)
    {
        try
        {
            var contacts = await _campusContactService.GetContactsByCampusAsync(campusId);
            
            if (!contacts.Any())
            {
                return NotFound(new { message = $"No se encontraron contactos para el campus {campusId}" });
            }

            return Ok(contacts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al obtener los contactos", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un contacto específico por su ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetContactById(int id)
    {
        try
        {
            var contact = await _campusContactService.GetContactByIdAsync(id);
            
            if (contact == null)
            {
                return NotFound(new { message = $"Contacto con ID {id} no encontrado" });
            }

            return Ok(contact);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al obtener el contacto", error = ex.Message });
        }
    }
}
