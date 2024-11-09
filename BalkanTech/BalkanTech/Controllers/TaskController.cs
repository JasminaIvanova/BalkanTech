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
        public IActionResult Index( int roomNumber, string category = "All")
        {
            var model = new TaskViewModel
            {
                RoomNumber = roomNumber,
                Categories = context.TaskCategories.Select(c => c.Name).ToList()
            };

            var room = context.Rooms
                .Include(r => r.MaintananceTasks)
                .ThenInclude(t => t.TaskCategory)
                .FirstOrDefault(r => r.RoomNumber == roomNumber);

            if (room == null)
            {
                return NotFound("Room not found.");
            }

            if (string.IsNullOrEmpty(category) || category == "All")
            {
                model.ToBeCompletedTasks = room.MaintananceTasks.Where(t => t.Status == "Pending").ToList();
                model.CompletedTasks = room.MaintananceTasks.Where(t => t.Status == "Completed").ToList();
            }
            else
            {
                model.ToBeCompletedTasks = room.MaintananceTasks
                    .Where(t => t.Status != "Completed" && t.TaskCategory != null && t.TaskCategory.Name == category)
                    .ToList();
                model.CompletedTasks = room.MaintananceTasks
                    .Where(t => t.Status == "Completed" && t.TaskCategory != null && t.TaskCategory.Name == category)
                    .ToList();
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
