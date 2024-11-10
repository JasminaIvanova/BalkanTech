using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using static BalkanTech.Common.Constants;
using static BalkanTech.Common.ErrorMessages.Rooms;

namespace BalkanTech.Web.Controllers
{
    //TODO -> validations , redirect after adding task, service for changing status, validation for correct task
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
            //TODO -> in service
            var task = await context.MaintananceTasks.FindAsync(id);
            if (task == null)
            {
                return Json(new { success = false, message = "Task not found" });
            }
            task.Status = newStatus;
            if (newStatus == "Completed")
            {
                task.CompletedDate = newDate ?? DateTime.Now;
            }

            await context.SaveChangesAsync();

            return Json(new { success = true, newStatus = newStatus, taskId = id, newDate = task.CompletedDate?.ToString("MM/dd/yyyy") });
        }

    }
}
