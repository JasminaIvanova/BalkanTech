using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Task;
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
            var model = await taskService.LoadTaskDetailsAsync(id);
            if (model == null) 
            {
                return NotFound("The task with the specified ID does not exist.");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
           
            var task = await context.MaintananceTasks.Include(t => t.AssignedTechniciansTasks).FirstOrDefaultAsync(t => t.Id == id);

            var model = new TaskAddViewModel
            {
                Name = task.Name,
                Description = task.Description,
                RoomId = task.RoomId,
                TaskCategoryId = task.TaskCategoryId,
                DueDate = task.DueDate.ToString(dateFormat),
                RoomNumbers = await taskService.LoadRoomsAsync(),
                Technicians = await taskService.LoadTechniciansAsync(),
                TaskCategories = await taskService.LoadTaskCategoriesAsync(),
                AssignedTechniciansIDs = task.AssignedTechniciansTasks.Select(at => at.AppUserId).ToList()
            };
            return View(model);
        }
    }
}
