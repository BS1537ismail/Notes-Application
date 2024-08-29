using Microsoft.EntityFrameworkCore;
using Notes_Application.Models.Entities;

namespace Notes_Application.Models.Data
{
    public class NotesDbContext : DbContext
    {
        public NotesDbContext(DbContextOptions<NotesDbContext> options):base(options)
        {
            
        }
        public DbSet<Note> Notes { get; set; }
    }
}
