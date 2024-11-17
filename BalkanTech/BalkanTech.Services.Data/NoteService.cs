using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Services.Data
{
    public class NoteService : INoteService
    {
        private readonly BalkanDbContext context;
        public NoteService(BalkanDbContext _context)
        {
            context = _context;
        }
        public async Task AddNoteAsync(string NoteComment, Guid TaskId, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(NoteComment) || TaskId == Guid.Empty)
            {
                throw new ArgumentException("Invalid note data.");
            }
            var newNote = new Note
            {
                NoteComment = NoteComment,
                MaintananceTaskId = TaskId,
                AppUserId = userId,
                NoteDate = DateTime.Now,
            };

            await context.Notes.AddAsync(newNote);
            await context.SaveChangesAsync();
        }
    }


}
