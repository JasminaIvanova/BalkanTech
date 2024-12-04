using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Report;
using BalkanTech.Web.ViewModels.Task;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace BalkanTech.Services.Data
{
    public class ReportService : IReportService
    {
        private readonly BalkanDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly ITaskService taskService;
        public ReportService(BalkanDbContext _context, UserManager<AppUser> _userManager, ITaskService _taskService)
        {
            context = _context;
            userManager = _userManager;
            taskService = _taskService;
        }
        public async Task<IEnumerable<TaskTechnicianViewModel>> GetAllTechUsersAsync()
        {
            return await taskService.LoadTechniciansAsync();
        }


        public async Task ChangeTaskStatus(Guid taskId, string newStatus, DateTime? newDate, string userId, bool isManager)
        {
            var task = await context.MaintananceTasks.Include(t => t.AssignedTechniciansTasks).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
            {
                throw new NullReferenceException("Task not found");
            }
            if (!Guid.TryParse(userId, out Guid parsedUserId))
            {
                throw new ArgumentException("Invalid user ID format.");
            }

            var assignedUser = task.AssignedTechniciansTasks.Any(att => att.AppUserId == parsedUserId && att.MaintananceTask.Id == taskId);

            if (assignedUser == false && isManager == false)
            {
                throw new UnauthorizedAccessException("You have no rights to change the status of this task as you are not assigned to it.");
            }
            if (newStatus == "Pending Manager Approval" && isManager == false)
            {
                task.Status = "Pending Manager Approval";
                task.CompletedDate = newDate ?? DateTime.Now;
            }
            else if (newStatus == "Completed" && isManager)
            {
                if (task.Status != "Pending Manager Approval")
                {
                    throw new InvalidOperationException("Task is not awaiting manager approval.");
                }
                task.Status = "Completed";
            }
            else
            {
                task.Status = newStatus;
            }

            await context.SaveChangesAsync();
        }
        public async Task<TasksPerTechnicianReportViewModel> GetTasksForUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out Guid parsedUserId))
            {
                throw new ArgumentException("Invalid or missing user.");
            }

            var tasks = await context.AssignedTechniciansTasks
                .Where(tt => tt.AppUserId == parsedUserId)
                .Include(tt => tt.MaintananceTask)
                    .ThenInclude(mt => mt.TaskCategory)
                .Include(tt => tt.MaintananceTask)
                    .ThenInclude(mt => mt.Room)
                .Select(tt => tt.MaintananceTask)
                .Where(mt => !mt.IsDeleted) 
                .ToListAsync();

            return new TasksPerTechnicianReportViewModel
            {
                UserId = parsedUserId,
                Name = $"{userManager.FindByIdAsync(userId).Result!.FirstName} {userManager.FindByIdAsync(userId).Result!.LastName}",
                ToBeCompletedTasks = tasks.Where(t => t.Status != "Completed").ToList(),
            };
        }
        }
}
