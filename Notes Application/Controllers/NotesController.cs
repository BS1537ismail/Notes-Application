using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        public async Task<IActionResult> GetAllNotes(string? search, string? lastRecordId, string? firstRecordId, int isPrevious, int pageSize = 5)
        {
            var notesQuery = context.Notes.AsQueryable();

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                notesQuery = notesQuery.Where(n =>
                    n.Title.ToLower().Contains(search) ||
                    n.Description.ToLower().Contains(search));
            }

            // Get total records before applying pagination
            var totalRecords = await notesQuery.CountAsync();

            if (!string.IsNullOrEmpty(lastRecordId) && isPrevious == 0)
            {
                if (Guid.TryParse(lastRecordId, out Guid parsedLastRecordId))
                {
                    notesQuery = notesQuery
                        .Where(n => n.Id.CompareTo(parsedLastRecordId) > 0)
                        .Take(pageSize); 
                }
                else
                {
                    return BadRequest("Invalid lastRecordId");
                }
            }
            else if (!string.IsNullOrEmpty(firstRecordId) && isPrevious == 1)
            {
                if (Guid.TryParse(firstRecordId, out Guid parsedFirstRecordId))
                {
                    notesQuery = notesQuery
                        .Where(n => n.Id.CompareTo(parsedFirstRecordId) < 0)
                        .OrderByDescending(n => n.Id)
                        .Take(pageSize);
                
                }
                else
                {
                    return BadRequest("Invalid lastRecordId");
                }
            }

            // Fetch the next set of records based on pageSize
            var paginatedNotes = await notesQuery
                .OrderBy(n => n.Id)
                .Take(pageSize)       // Fetch the next 'pageSize' number of records
                .ToListAsync();

            // Reverse the result if fetching previous data to display it in correct order
            //if (isPrevious)
            //{
            //    paginatedNotes.Reverse();
            //}

            return Ok(new
            {
                data = paginatedNotes,
                totalRecords,
                pageSize
            });
        }



        [HttpGet("{id}")]
       // [Route("{id:Guid}")]
        //[ActionName("GetNoteById")]
        public async Task<IActionResult> GetNoteById([FromRoute] string id)
        {
            // Convert id from string to Guid
            if (!Guid.TryParse(id, out Guid noteId))
            {
                return BadRequest("Invalid note ID");
            }
            var data = await context.Notes.FindAsync(noteId);
            if (data == null) return NotFound();
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> AddNote(Note note)
        {
            if (ModelState.IsValid && note.Title != "" && note.Description != "")
            {
                note.Id = Guid.NewGuid();
                await context.AddAsync(note);
                await context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetNoteById), new { id = note.Id.ToString() }, note);
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote([FromRoute] string id, [FromBody] Note updatedNote)
        {
            if (!Guid.TryParse(id, out Guid noteId))
            {
                return BadRequest("Invalid note ID");
            }

            var data = await context.Notes.FindAsync(noteId);
            if (data == null) return NotFound();

            if(!ModelState.IsValid || updatedNote == null) return BadRequest();
            data.Title = updatedNote.Title;
            data.Description = updatedNote.Description; 
            data.IsVisible = updatedNote.IsVisible; 

            await context.SaveChangesAsync();

            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote([FromRoute] string id)
        {
            // Convert id from string to Guid
            if (!Guid.TryParse(id, out Guid noteId))
            {
                return BadRequest("Invalid note ID");
            }

            var data = await context.Notes.FindAsync(noteId);
            if(data == null)return NotFound();

            context.Notes.Remove(data);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
