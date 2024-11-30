using BalkanTech.Data;
using BalkanTech.Services.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        public async Task ChangeTaskStatus(Guid taskId, string newStatus, DateTime? newDate)
        {
            var task = await context.MaintananceTasks.FindAsync(taskId);
            if (task == null)
            {
                throw new NullReferenceException("Task not found");
            }
            task.Status = newStatus;
            if (newStatus == "Completed")
            {
                task.CompletedDate = newDate ?? DateTime.Now;
            }

            await context.SaveChangesAsync();
        }
    }
}
