using BalkanTech.Data;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Report;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Services.Data
{
    public class ReportService : IReportService
    {
        private readonly BalkanDbContext context;
        public ReportService(BalkanDbContext _context)
        {
            context = _context;
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

            if (assignedUser == false || isManager == false)
            {
                throw new UnauthorizedAccessException("You have no rights to change the status of this task as you are not assigned to it.");
            }
            task.Status = newStatus;
            if (newStatus == "Completed")
            {
                task.CompletedDate = newDate ?? DateTime.Now;
            }

            await context.SaveChangesAsync();
        }

        public async Task<TasksPerTechnicianReportViewModel> ListAssignedTasks(string userId)
        {
            if (!Guid.TryParse(userId, out Guid parsedUserId))
            {
                throw new ArgumentException("Invalid user ID format.");
            }

            var tasksForUser = await context.AssignedTechniciansTasks
                .Where(tt => tt.AppUserId == parsedUserId)
                .Include(tt => tt.MaintananceTask)
                    .ThenInclude(mt => mt.Room)
                .Include(tt => tt.MaintananceTask)
                    .ThenInclude(mt => mt.TaskCategory)
                .Select(tt => tt.MaintananceTask)
                .Where(mt => mt.Status != "Completed" && !mt.IsDeleted)
                .ToListAsync();

            if (tasksForUser == null)
            {
                throw new InvalidOperationException("An error occured while trying to retrieve data for assigned tasks");
            }

            return new TasksPerTechnicianReportViewModel
            {
                ToBeCompletedTasks = tasksForUser
            };

        }
    }
}
