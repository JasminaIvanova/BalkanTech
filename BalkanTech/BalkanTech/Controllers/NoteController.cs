using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Web.ViewModels.Note;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BalkanTech.Web.Controllers
{
    public class NoteController : Controller
    {
        private readonly BalkanDbContext context;
        public NoteController(BalkanDbContext _context)
        {
            context = _context;
        }

        [HttpPost]
        public async Task<IActionResult> Add(string NoteComment, Guid TaskId)
        {
            if (string.IsNullOrWhiteSpace(NoteComment) || TaskId == Guid.Empty)
            {
                TempData["Error"] = "Invalid note data.";
                return RedirectToAction("TaskDetails", "Task", new { id = TaskId });
            }
            var newNote = new Note
            {
                NoteComment = NoteComment,
                MaintananceTaskId = TaskId,
                AppUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                NoteDate = DateTime.Now,
            };

            await context.Notes.AddAsync(newNote);
            await context.SaveChangesAsync();
            return RedirectToAction("TaskDetails", "Task", new { id = TaskId });

        }
    }
}
