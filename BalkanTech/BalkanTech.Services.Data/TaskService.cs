using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Note;
using BalkanTech.Web.ViewModels.Task;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BalkanTech.Common.Constants;

namespace BalkanTech.Services.Data
{
    //fix validations, errors, handle exceptions, simplify remove repeting code, fix bmapping 
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
            var allTechnicians = await userManager.GetUsersInRoleAsync("Technician");
            var allManagers = await userManager.GetUsersInRoleAsync("Manager");
            if (allTechnicians == null || allManagers == null)
            {
                throw new NullReferenceException("Unable to load users to be assigned");
            }
            var techsAndManager = allTechnicians.Concat(allManagers).ToList();
            return techsAndManager.Select(t => new TaskTechnicianViewModel
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
            if (model == null) 
            {
                throw new NullReferenceException("Model is empty");
            }
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

        public async Task<TaskViewModel> IndexGetAllTasksAsync(Guid roomId, int roomNumber,int? completedPage, int? toBeCompletedPage, int pageSize, string category = "All")
        {
            if (roomId == Guid.Empty || roomNumber <= MinValueRoomNumber) 
            {
                throw new ArgumentException("Invalid room details");
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


            if (room == null)
            {
                throw new NullReferenceException($"Room not found.");
            }


            var tasks = room.MaintananceTasks.Where(t => t.IsDeleted == false).AsQueryable();
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                tasks = tasks.Where(t => t.TaskCategory != null && t.TaskCategory.Name == category);
            }
            var toBeCompletedTasks = tasks.Where(t => t.Status != "Completed").ToList();
            var completedTasks = tasks.Where(t => t.Status == "Completed").ToList();

            model.ToBeCompletedTasks.TotalItems = toBeCompletedTasks.Count();
            model.ToBeCompletedTasks.TotalPages = (int)Math.Ceiling(toBeCompletedTasks.Count() / (double)pageSize);
            model.ToBeCompletedTasks.PageSize = pageSize;
            model.ToBeCompletedTasks.Items = toBeCompletedTasks
                .Skip(((toBeCompletedPage ?? 1) - 1) * pageSize)  
                .Take(pageSize)
                .ToList();
            model.ToBeCompletedTasks.CurrentPage = toBeCompletedPage ?? 1;

            model.CompletedTasks.TotalItems = completedTasks.Count();
            model.CompletedTasks.TotalPages = (int)Math.Ceiling(completedTasks.Count() / (double)pageSize);
            model.CompletedTasks.PageSize = pageSize;
            model.CompletedTasks.Items = completedTasks
                .Skip(((completedPage ?? 1) - 1) * pageSize) 
                .Take(pageSize)
                .ToList();
            model.CompletedTasks.CurrentPage = completedPage ?? 1;
            

            return model;
        }

        

        public async Task<TaskDetailsViewModel> LoadTaskDetailsAsync(Guid id)
        {
            var task = await context.MaintananceTasks
                .Where(t => t.IsDeleted == false)
                 .Include(t => t.Room)
                 .Include(t => t.Notes)
                    .ThenInclude(n => n.AppUser)
                 .Include(t => t.TaskCategory)
                 .Include(t => t.AssignedTechniciansTasks)
                     .ThenInclude(at => at.AppUser)
                 .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                throw new NullReferenceException("Task not found");
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
           var task = await context.MaintananceTasks.Where(t => t.IsDeleted == false).Include(t => t.AssignedTechniciansTasks).Include(t => t.Room).FirstOrDefaultAsync(t => t.Id == id);
            if (task != null) 
            {
                return task;
            }
            throw new NullReferenceException("Task not found");
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
            if (model == null)
            {
                throw new ArgumentNullException("Model is null");
            }
            var task = await LoadMaintananceTaskAsync(model.Id);
     
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

        public async Task<TaskDeleteViewModel> LoadDeleteViewModelAsync(Guid id)
        {
            var task = await LoadMaintananceTaskAsync(id);
            var model = new TaskDeleteViewModel
            {

                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                RoomNumber = task.Room.RoomNumber,
                RoomId = task.RoomId,
            };
            return model;
        }

        public async Task<bool> DeleteTaskAsync(TaskDeleteViewModel model)
        {
            var task = await LoadMaintananceTaskAsync(model.Id);
            task.IsDeleted = true;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
