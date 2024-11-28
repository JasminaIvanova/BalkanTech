using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Web.ViewModels.Report;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BalkanTech.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly BalkanDbContext context;
        private readonly UserManager<AppUser> userManager;
        public ReportsController(BalkanDbContext _context, UserManager<AppUser> _userManager)
        {
            context = _context;
            userManager = _userManager;
        }
        [HttpGet]
        public async Task<IActionResult> MyTasks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userId, out Guid parsedUserId))
            {
                var tasksForUser = await context.AssignedTechniciansTasks.Where(tt => tt.AppUserId == parsedUserId)
                    .Include(t => t.MaintananceTask)
                    .ThenInclude(t => t.Room)
                    .Include(t => t.MaintananceTask)
                    .ThenInclude(t => t.TaskCategory).Select(t => t.MaintananceTask).Where(t => t.Status != "Completed" && t.IsDeleted == false).ToListAsync();
                var model = new TasksPerTechnicianReportViewModel
                {
                    ToBeCompletedTasks = tasksForUser

                };
                return View(model);
            }
            else
            {
                return NotFound();
            }
           
        }
    }
}
