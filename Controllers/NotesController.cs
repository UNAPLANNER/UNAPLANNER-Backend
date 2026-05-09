using Microsoft.AspNetCore.Mvc;
using UNAPLANNER_API.DTOs.Requests;
using UNAPLANNER_API.Services;

namespace UNAPLANNER_API.Controllers;

[Route("api/notes")]
[ApiController]
public class NotesController : ControllerBase
{
    private readonly INotesService _notesService;

    public NotesController(INotesService notesService)
    {
        _notesService = notesService;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteRequest request)
    {
        var result = await _notesService.UpdateNoteAsync(id, request);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Note);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(int id, [FromQuery] int userId)
    {
        var result = await _notesService.DeleteNoteAsync(id, userId);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return NoContent();
    }
}
