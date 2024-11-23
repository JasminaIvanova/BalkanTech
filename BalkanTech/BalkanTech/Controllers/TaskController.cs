using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;
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
                Id = id,
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
        [HttpPost]
        public async Task<IActionResult> Edit(TaskAddViewModel model)
        {
            var task = await context.MaintananceTasks.Include(t => t.AssignedTechniciansTasks).FirstOrDefaultAsync(t => t.Id == model.Id);
            if (task == null)
            {
                return NotFound(); 
            }
            if (!DateTime.TryParseExact(model.DueDate, dateFormat, CultureInfo.InvariantCulture,
              DateTimeStyles.None, out DateTime parsedDueDate))
            {
                throw new InvalidOperationException("Invalid date format.");
            }
           
            task.Name = model.Name;
            task.Description = model.Description;
            task.RoomId = model.RoomId;
            task.TaskCategoryId = model.TaskCategoryId;
            task.DueDate = parsedDueDate;

            var assignedTechsBeforeEdit = task.AssignedTechniciansTasks.Select(at => at.AppUserId).ToList();
            var techsToRemoveAfterEdit = assignedTechsBeforeEdit.Except(model.AssignedTechniciansIDs).ToList();
            var techsToAdd = model.AssignedTechniciansIDs.Except(assignedTechsBeforeEdit).ToList();

            var removeTechsAssigned = task.AssignedTechniciansTasks
                .Where(t => techsToRemoveAfterEdit.Contains(t.AppUserId)).ToList();
            context.AssignedTechniciansTasks.RemoveRange(removeTechsAssigned);

            var techsAssignedAdd = techsToAdd
                            .Select(techId => new AssignedTechnicianTask
                            {
                                AppUserId = techId,
                                MaintananceTaskId = task.Id
                            })
                            .ToList();
            await context.AssignedTechniciansTasks.AddRangeAsync(techsAssignedAdd);
            await context.SaveChangesAsync();
            return RedirectToAction("TaskDetails", new { id = task.Id });
            
        }
     }
}
