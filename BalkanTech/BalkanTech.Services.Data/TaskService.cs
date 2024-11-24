using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Note;
using BalkanTech.Web.ViewModels.Task;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static BalkanTech.Common.Constants;

namespace BalkanTech.Services.Data
{
    //fix validations, errors, handle exceptions, simplify remove repeting code, fix mapping 
    public class TaskService : ITaskService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly BalkanDbContext context;
        public TaskService(BalkanDbContext _context, UserManager<AppUser> _userManager)
        {
            context = _context;
            userManager = _userManager;
        }
        public async Task<IEnumerable<TaskTechnicianViewModel>> LoadTechniciansAsync()
        {
            var allTechs = await userManager.GetUsersInRoleAsync("Technician");
            return allTechs.Select(t => new TaskTechnicianViewModel
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
            var rooms = await LoadRoomsAsync();
        }

        public async Task<TaskViewModel> IndexGetAllTasksAsync(Guid roomId, int roomNumber, string category = "All")
        {
            if (!context.Rooms.Any(r => r.RoomNumber == roomNumber)) 
            {
                throw new InvalidOperationException();

            }
            var model = new TaskViewModel
            {
                RoomId = roomId,
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

        public async Task<TaskDetailsViewModel> LoadTaskDetailsAsync(Guid id)
        {
            var task = await context.MaintananceTasks
                 .Include(t => t.Room)
                 .Include(t => t.Notes)
                    .ThenInclude(n => n.AppUser)
                 .Include(t => t.TaskCategory)
                 .Include(t => t.AssignedTechniciansTasks)
                     .ThenInclude(at => at.AppUser)
                 .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return null;
            }
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
                AssignedTechnicians = task.AssignedTechniciansTasks.Select(tt => new TaskTechnicianViewModel
                {
                    Id = tt.AppUserId,
                    FirstName = tt.AppUser.FirstName,
                    LastName = tt.AppUser.LastName,
                }).ToList(),
                Notes = task.Notes.Select(n => new NotesViewModel
                {
                    Id = n.Id,
                    NoteComment = n.NoteComment,
                    NoteDate = n.NoteDate,
                    AppUserName = $"{n.AppUser?.FirstName} {n.AppUser?.LastName}"
                }).OrderByDescending(n => n.NoteDate).ToList(),
            };
            return model;
        }

        public async Task<MaintananceTask> LoadMaintananceTaskAsync(Guid id)
        {
           var task = await context.MaintananceTasks.Include(t => t.AssignedTechniciansTasks).FirstOrDefaultAsync(t => t.Id == id);
            if (task != null) 
            {
                return task;
            }
            throw new InvalidOperationException();
        }

        public async Task<TaskAddViewModel> LoadEditTaskAsync(Guid id)
        {
            var task = await LoadMaintananceTaskAsync(id);

            var model = new TaskAddViewModel
            {
                Id = id,
                Name = task.Name,
                Description = task.Description,
                RoomId = task.RoomId,
                TaskCategoryId = task.TaskCategoryId,
                DueDate = task.DueDate.ToString(dateFormat),
                RoomNumbers = await LoadRoomsAsync(),
                Technicians = await LoadTechniciansAsync(),
                TaskCategories = await LoadTaskCategoriesAsync(),
                AssignedTechniciansIDs = task.AssignedTechniciansTasks.Select(at => at.AppUserId).ToList()
            };  
            return model;
        }

        public async Task<TaskAddViewModel> LoadTaskAddModel(Guid roomId)
        {
           return new TaskAddViewModel()
           {
               RoomId = roomId,
               RoomNumbers = await LoadRoomsAsync(),
               Technicians = await LoadTechniciansAsync(),
               TaskCategories = await LoadTaskCategoriesAsync(),
           };
        }

        public async Task EditTaskAsync(TaskAddViewModel model, DateTime parsedDate)
        {
            var task = await LoadMaintananceTaskAsync(model.Id);
            if (task == null)
            {
                throw new InvalidOperationException();
            }
     
            task.Name = model.Name;
            task.Description = model.Description;
            task.RoomId = model.RoomId;
            task.TaskCategoryId = model.TaskCategoryId;
            task.DueDate = parsedDate;

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
        }

        public async Task<int> GetRoomNumberByIdAsync(Guid roomId)
        {
            return await context.Rooms
                .Where(r => r.Id == roomId)
                .Select(r => r.RoomNumber)
                .FirstOrDefaultAsync();
        }
    }
}
