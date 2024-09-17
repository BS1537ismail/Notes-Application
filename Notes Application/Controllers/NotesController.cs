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

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetNoteById([FromRoute] Guid id)
    {
        var note = await _notesService.GetNoteByIdAsync(id);
        if (note == null) return NotFound();
        return Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> AddNote(Note note)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(note.Title) || string.IsNullOrWhiteSpace(note.Description))
            return BadRequest();

        await _notesService.AddNoteAsync(note);
        return CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note);
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> UpdateNote([FromRoute] Guid id, [FromBody] Note updatedNote)
    {
        var existingNote = await _notesService.GetNoteByIdAsync(id);
        if (existingNote == null) return NotFound();

        existingNote.Title = updatedNote.Title;
        existingNote.Description = updatedNote.Description;
        existingNote.IsVisible = updatedNote.IsVisible;

        await _notesService.UpdateNoteAsync(existingNote);
        return Ok(existingNote);
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> DeleteNote([FromRoute] Guid id)
    {
        var note = await _notesService.GetNoteByIdAsync(id);
        if (note == null) return NotFound();

        await _notesService.DeleteNoteAsync(id);
        return Ok();
    }
}
