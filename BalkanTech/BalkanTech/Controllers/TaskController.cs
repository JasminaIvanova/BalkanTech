using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BalkanTech.Web.Controllers
{
    //TODO -> service, validations, async, fix logic to be more clean
    public class TaskController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly BalkanDbContext context;
        public TaskController(BalkanDbContext _context, UserManager<AppUser> userManager)
        {
            context = _context;
            _userManager = userManager;
         
        }
        [HttpGet]
        public IActionResult Index(Guid id, int roomNumber, string category)
        {
            
            var model = new TaskViewModel();
            model.RoomNumber = roomNumber; 
            var room = context.Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            model.Categories = context.TaskCategories.Select(c => c.Name).ToList();
            if (category == "All")
            {
                model.ToBeCompletedTasks = room.MaintananceTasks.Where(t => t.Status != "Completed").ToList();
                model.CompletedTasks = room.MaintananceTasks.Where(t => t.Status == "Completed").ToList(); ;
            }
            else
            {
                model.ToBeCompletedTasks = room.MaintananceTasks.Where(t => t.Status != "Completed" && t.TaskCategory.Name == category).ToList();
                model.CompletedTasks = room.MaintananceTasks.Where(t => t.Status == "Completed" && t.TaskCategory.Name == category).ToList(); ;
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new TaskAddViewModel();
            model.RoomNumbers = context.Rooms.Select(r => r.RoomNumber).ToList();
            model.Technicians = (await _userManager.GetUsersInRoleAsync("Technician"))
                            .Select(t => t.FirstName)
                            .ToList();
            model.TaskCategories = context.TaskCategories.Select(c => c.Name).ToList();
            return View(model);
        }

     }
}
