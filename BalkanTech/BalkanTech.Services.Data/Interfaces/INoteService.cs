using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Services.Data.Interfaces
{
    public interface INoteService
    {
        Task AddNoteAsync(string NoteComment, Guid TaskId, Guid userId);
    }
}
