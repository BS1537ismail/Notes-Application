using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes_Application.Models.Data;
using Notes_Application.Models.Entities;

namespace Notes_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly NotesDbContext context;

        public NotesController(NotesDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllNotes(string? search, int pageNumber = 1, int pageSize = 8)
        {
            var notesQuery = context.Notes.AsQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                notesQuery = notesQuery.Where(n =>
                    n.Title.ToLower().Contains(search) ||
                    n.Description.ToLower().Contains(search));
            }

            // Apply pagination
            var totalRecords = await notesQuery.CountAsync(); 
            var paginatedNotes = await notesQuery
                .Skip((pageNumber - 1) * pageSize) 
                .Take(pageSize)  
                .ToListAsync();

            return Ok(new
            {
                totalRecords,
                pageNumber,
                pageSize,
                data = paginatedNotes
            });
        }


        [HttpGet]
        [Route("{id:Guid}")]
        //[ActionName("GetNoteById")]
        public async Task<IActionResult> GetNoteById([FromRoute] Guid id)
        {
            var data = await context.Notes.FindAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> AddNote(Note note)
        {
            if (ModelState.IsValid)
            {
                note.Id = Guid.NewGuid();
                await context.AddAsync(note);
                await context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note);
            }
            return BadRequest();
        }
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateNote([FromRoute] Guid id, [FromBody] Note updatedNote)
        {
            var data = await context.Notes.FindAsync(id);
            if (data == null) return NotFound();

            if(!ModelState.IsValid || updatedNote == null) return BadRequest();
            data.Title = updatedNote.Title;
            data.Description = updatedNote.Description; 
            data.IsVisible = updatedNote.IsVisible; 

            await context.SaveChangesAsync();

            return Ok(data);
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteNote([FromRoute] Guid id)
        {
            var data = await context.Notes.FindAsync(id);
            if(data == null)return NotFound();

            context.Notes.Remove(data);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
