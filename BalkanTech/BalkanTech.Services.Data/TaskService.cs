using BalkanTech.Data.Models;
using BalkanTech.Data;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BalkanTech.Services.Data
{
    public class TaskService : ITaskService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly BalkanDbContext context;
        public TaskService(BalkanDbContext _context, UserManager<AppUser> _userManager)
        {
            context = _context;
            userManager = _userManager;
        }
        public async Task<IEnumerable<TaskAddTechnicianViewModel>> LoadTechniciansAsync()
        {
            var allTechs = await userManager.GetUsersInRoleAsync("Technician");
            return allTechs.Select(t => new TaskAddTechnicianViewModel
            {
                Id = t.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
            }).ToList();

        }
        public async Task<IEnumerable<TaskAddRoomViewModel>> LoadRoomsAsync()
        {
            return await context.Rooms.Select(r => new TaskAddRoomViewModel
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
            }).ToListAsync();
        }

        public async Task<IEnumerable<TaskCategoryViewModel>> LoadTaskCategoriesAsync()
        {
            return await context.TaskCategories.Select(tc => new TaskCategoryViewModel
            {
                Id = tc.Id,
                TaskCategoryName = tc.Name
            }).ToListAsync();
        }
        public async Task AddTaskAsync(TaskAddViewModel model, DateTime parsedDueDate)
        {
            MaintananceTask myTask = new MaintananceTask
            {
                Name = model.Name,
                Description = model.Description,
                RoomId = model.RoomId,
                TaskCategoryId = model.TaskCategoryId,
                DueDate = parsedDueDate,
            };
            await context.MaintananceTasks.AddAsync(myTask);
            var tasksAssigned = model.AssignedTechniciansIDs
                              .Select(techId => new AssignedTechnicianTask
                              {
                                  AppUserId = techId,
                                  MaintananceTaskId = myTask.Id
                              })
                              .ToList();
            await context.AssignedTechniciansTasks.AddRangeAsync(tasksAssigned);
            await context.SaveChangesAsync();
        }

        public async Task<TaskViewModel> IndexGetAllTasksAsync(int roomNumber, string category = "All")
        {
            if (!context.Rooms.Any(r => r.RoomNumber == roomNumber)) 
            {
                throw new InvalidOperationException();

            }
            var model = new TaskViewModel
            {
                RoomNumber = roomNumber,
                Categories = await context.TaskCategories.Select(c => c.Name).ToListAsync()
            };

            var room = await context.Rooms
                .Include(r => r.MaintananceTasks)
                .ThenInclude(t => t.TaskCategory)
                .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);

            if (room != null)
            {
                var tasks = room.MaintananceTasks.AsQueryable();
                if (!string.IsNullOrEmpty(category) && category != "All")
                {
                    tasks = tasks.Where(t => t.TaskCategory != null && t.TaskCategory.Name == category);
                }
                model.ToBeCompletedTasks = tasks.Where(t => t.Status != "Completed").ToList();
                model.CompletedTasks = tasks.Where(t => t.Status == "Completed").ToList();
            }
            return model;
        }

        public async Task<JsonResult> ChangeTaskStatus(Guid id, string newStatus, DateTime? newDate)
        {
            var task = await context.MaintananceTasks.FindAsync(id);
            if (task == null)
            {
                return new JsonResult(new { success = false, message = "Task not found" });
            }
            task.Status = newStatus;
            if (newStatus == "Completed")
            {
                task.CompletedDate = newDate ?? DateTime.Now;
            }

            await context.SaveChangesAsync();

            return new JsonResult(new { success = true, newStatus = newStatus, taskId = id, newDate = task.CompletedDate?.ToString("MM/dd/yyyy") });
        }
    }
}
