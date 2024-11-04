using BalkanTech.Data;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BalkanTech.Web.Controllers
{
    public class TaskController : Controller
    {
        private readonly BalkanDbContext context;
        public TaskController(BalkanDbContext _context)
        {
            context = _context;
        }
        [HttpGet]
        public IActionResult Index(Guid id, int roomNumber)
        {
            //TODO -> service
            var model = new TaskViewModel();
            model.RoomNumber = roomNumber; 
            model.Categories = context.TaskCategories.Select(c => c.Name).ToList();
            return View(model);
        }
    }
}
