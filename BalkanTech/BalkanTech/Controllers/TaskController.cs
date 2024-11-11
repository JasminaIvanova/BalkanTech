using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static BalkanTech.Common.Constants;
using static BalkanTech.Common.ErrorMessages.Rooms;

namespace BalkanTech.Web.Controllers
{
    //TODO -> validations , redirect after adding task, validation for correct task, pages for tasks
    [Authorize]
    public class TaskController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITaskService taskService;
        private readonly BalkanDbContext context;
        public TaskController(BalkanDbContext _context, UserManager<AppUser> userManager, ITaskService _taskService)
        {
            context = _context;
            _userManager = userManager;
            taskService = _taskService;
         
        }
        [HttpGet]
        public async Task<IActionResult> Index( Guid roomId, int roomNumber,string category = "All")
        {
            try
            {
                var model = await taskService.IndexGetAllTasksAsync(roomNumber, category);
                if (model == null)
                {
                    return NotFound("Room not found.");
                }
                return View(model);
            }
            catch (InvalidOperationException roomEx)
            {
               
                TempData[nameof(ErrorRoomNumber)] = ErrorRoomNumber;
                return RedirectToAction("Index", "Room");
            }

        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new TaskAddViewModel()
            {
                RoomNumbers = await taskService.LoadRoomsAsync(),
                Technicians = await taskService.LoadTechniciansAsync(),
                TaskCategories = await taskService.LoadTaskCategoriesAsync(),
            };
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TaskAddViewModel model)
        {
            if (!ModelState.IsValid) 
            {
                model.RoomNumbers = await taskService.LoadRoomsAsync();
                model.Technicians = await taskService.LoadTechniciansAsync();
                model.TaskCategories = await taskService.LoadTaskCategoriesAsync();
                return View(model);
            }
            if (!DateTime.TryParseExact(model.DueDate, dateFormat, CultureInfo.InvariantCulture,
               DateTimeStyles.None, out DateTime parsedDueDate))
            {
                throw new InvalidOperationException("Invalid date format.");
            }
           await taskService.AddTaskAsync(model, parsedDueDate);
           return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<IActionResult> ChangeTaskStatus(Guid id, string newStatus, DateTime? newDate)
        {
            return await taskService.ChangeTaskStatus(id, newStatus, newDate);  
        }

        [HttpGet]
        public async Task<IActionResult> TaskDetails(Guid id)
        {
            var task = await context.MaintananceTasks
                .Include(t => t.Room)
                .Include(t => t.TaskCategory)
                .Include(t => t.AssignedTechniciansTasks) 
                    .ThenInclude(at => at.AppUser)
                .FirstOrDefaultAsync(t => t.Id == id);
            var model = new TaskDetailsViewModel()
            {
                Id = id,
                Name = task.Name,
                Description = task.Description,
                RoomNumber = task.Room.RoomNumber,
                DueDate = task.DueDate,
                CompletedDate = task.CompletedDate,
                Status = task.Status,
                TaskCategory = task.TaskCategory.Name,
                AssignedTechnicians = task.AssignedTechniciansTasks.Select(tt => new TaskAddTechnicianViewModel
                {
                    Id = tt.AppUserId,
                    FirstName = tt.AppUser.FirstName,
                    LastName = tt.AppUser.LastName,
                }).ToList()
            };
            return View(model);
        }
    }
}
