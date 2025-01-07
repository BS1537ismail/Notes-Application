using Nest;
using Notes_Application.Models.Entities;
public class NoteService(IRepository<Note> noteRepository, IElasticClient elasticClient) : INoteService
{
    private readonly IRepository<Note> _noteRepository = noteRepository;

    public async Task<dynamic> GetAllNotesAsync(string? search, int pageNumber, int pageSize)
    {
        var notesQuery = await _noteRepository.GetAllAsync();
        var totalRecords = notesQuery.Count();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            notesQuery = notesQuery.Where(n =>
                n.Title.ToLower().Contains(search) ||
                n.Description.ToLower().Contains(search));
        }

        var paginatedNotes = notesQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(n => n.Id);

        return new
        {
            Notes = paginatedNotes,
            TotalRecords = totalRecords
        };
    }
    public async Task<Note> GetNoteByIdAsync(string id)
    {
        return await _noteRepository.GetByIdAsync(id);
    }

    public async Task AddNoteAsync(Note note)
    {
        note.Id = Guid.NewGuid();
        await _noteRepository.AddAsync(note);
    }

    public async Task UpdateNoteAsync(Note note, Note updatedNote)
    {
        note.Title = updatedNote.Title;
        note.Description = updatedNote.Description;
        note.IsVisible = updatedNote.IsVisible;
        await _noteRepository.UpdateAsync(note);
    }

    public async Task DeleteNoteAsync(string id)
    {
        await _noteRepository.DeleteAsync(id);
    }

    //public Task<int> GetTotalRecord()
    //{
    //    return _noteRepository.CountAsync();
    //}
}
