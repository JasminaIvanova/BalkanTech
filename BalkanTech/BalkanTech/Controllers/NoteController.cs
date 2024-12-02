using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Note;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BalkanTech.Web.Controllers
{
    public class NoteController : Controller
    {
        private readonly INoteService noteService;
        public NoteController(INoteService _noteService)
        {
            noteService = _noteService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(string noteComment, Guid taskId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await noteService.AddNoteAsync(noteComment, taskId, userId); 
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("TaskDetails", "Task", new { id = taskId });
        }
    }
}
