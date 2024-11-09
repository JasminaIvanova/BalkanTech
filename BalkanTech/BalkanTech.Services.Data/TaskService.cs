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

namespace BalkanTech.Services.Data
{
    public class TaskService : ITaskService
    {
        private readonly BalkanDbContext context;
        public TaskService(BalkanDbContext _context)
        {
            context = _context;
        }
        public Task<bool> AddTaskAsync(TaskAddViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<TaskViewModel> IndexGetAllTasksAsync(int roomNumber, string category = "All")
        {
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
    }
}
