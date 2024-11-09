using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Globalization;
using static BalkanTech.Common.Constants;
using static BalkanTech.Common.ErrorMessages;

namespace BalkanTech.Web.Controllers
{
    //TODO -> service, validations, async, fix logic to be more clean
    public class TaskController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITaskService taskSerrvice;
        private readonly BalkanDbContext context;
        public TaskController(BalkanDbContext _context, UserManager<AppUser> userManager, ITaskService _taskService)
        {
            context = _context;
            _userManager = userManager;
            taskSerrvice = _taskService;
         
        }
        [HttpGet]
        public async Task<IActionResult> Index( int roomNumber, string category = "All")
        {
            var model = await taskSerrvice.IndexGetAllTasksAsync(roomNumber, category);
            if (model == null)
            {
                return NotFound("Room not found.");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new TaskAddViewModel();
            model.RoomNumbers = context.Rooms.Select(r => new TaskAddRoomViewModel
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
            });
            model.Technicians = (await _userManager.GetUsersInRoleAsync("Technician"))
                            .Select(t => new TaskAddTechnicianViewModel
                            {
                                Id = t.Id,
                                FirstName = t.FirstName
                            }).ToList();
            model.TaskCategories = context.TaskCategories.Select(tc => new TaskCategoryViewModel
            {
                Id = tc.Id,
                TaskCategoryName = tc.Name
            });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TaskAddViewModel model)
        {
            if (!DateTime.TryParseExact(model.DueDate, dateFormat, CultureInfo.InvariantCulture,
               DateTimeStyles.None, out DateTime parsedDueDate))
            {
                throw new InvalidOperationException("Invalid date format.");
            }
            if (!ModelState.IsValid) 
            {
                model.RoomNumbers = context.Rooms.Select(r => new TaskAddRoomViewModel
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                });
                model.Technicians = (await _userManager.GetUsersInRoleAsync("Technician"))
                           .Select(t => new TaskAddTechnicianViewModel
                           {
                               Id = t.Id,
                               FirstName = t.FirstName
                           }).ToList();
                model.TaskCategories = context.TaskCategories.Select(tc => new TaskCategoryViewModel
                {
                    Id = tc.Id,
                    TaskCategoryName = tc.Name
                });
                return View(model);
            }
            

            MaintananceTask myTask = new MaintananceTask
            {
                Description = model.Description,
                RoomId = model.RoomId,
                TaskCategoryId = model.TaskCategoryId,
                DueDate = parsedDueDate,
            };
            context.MaintananceTasks.Add(myTask);
            var tasksAssigned = model.AssignedTechniciansIDs
                              .Select(techId => new AssignedTechnicianTask
                              {
                                  AppUserId = techId,
                                  MaintananceTaskId = myTask.Id
                              })
                              .ToList();
            context.AssignedTechniciansTasks.AddRange(tasksAssigned);
            await context.SaveChangesAsync();
            return View();

        }

    }
}
