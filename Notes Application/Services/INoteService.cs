using Notes_Application.Models.Entities;

public interface INoteService
{
    Task<dynamic> GetAllNotesAsync(string search, int pageNumber, int pageSize);
    Task<Note> GetNoteByIdAsync(string id);
    Task AddNoteAsync(Note note);
    Task UpdateNoteAsync(Note note, Note updatedNote);
    Task DeleteNoteAsync(string id);
    //Task<int> GetTotalRecord();
}
