using Microsoft.AspNetCore.Mvc;
using Notes_Application.Models.Entities;

[Route("api/[controller]")]
[ApiController]
public class NotesController : ControllerBase
{
    private readonly INoteService _notesService;

    public NotesController(INoteService notesService)
    {
        _notesService = notesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllNotes(string? search, int pageNumber = 1, int pageSize = 8)
    {
        var result = await _notesService.GetAllNotesAsync(search, pageNumber, pageSize);

        return Ok(new
        {
            totalRecords = result.TotalRecords,
            pageNumber,
            pageSize,
            data = result.Notes
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNoteById(string id)
    {
        var note = await _notesService.GetNoteByIdAsync(id);
        if (note == null) return NotFound();
        return Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> AddNote(Note note)
    {
        if (ModelState.IsValid) 
        {
            await _notesService.AddNoteAsync(note);

            return CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note);
        }

        return BadRequest(); 
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(string id, [FromBody] Note updatedNote)
    {
        var existingNote = await _notesService.GetNoteByIdAsync(id);

        await _notesService.UpdateNoteAsync(existingNote, updatedNote);

        return Ok(existingNote);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(string id)
    {
        var note = await _notesService.GetNoteByIdAsync(id);

        await _notesService.DeleteNoteAsync(id);

        return Ok();
    }
}
