using Notes_Application.Models.Entities;

public interface INoteService
{
    Task<dynamic> GetAllNotesAsync(string search, int pageNumber, int pageSize);
    Task<Note> GetNoteByIdAsync(Guid id);
    Task AddNoteAsync(Note note);
    Task UpdateNoteAsync(Note note);
    Task DeleteNoteAsync(Guid id);
    Task<int> GetTotalRecord();
}
