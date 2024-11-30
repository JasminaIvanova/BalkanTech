using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static BalkanTech.Common.ErrorMessages.Tasks;

namespace BalkanTech.Web.Controllers
{
    [Authorize]
    [Authorize(Roles = "Technician, Manager")]
    public class ReportsController : Controller
    {
        private readonly BalkanDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly IReportService reportService;
        public ReportsController(BalkanDbContext _context, UserManager<AppUser> _userManager, IReportService _reportService)
        {
            context = _context;
            userManager = _userManager;
            reportService = _reportService;
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
        [HttpPost]
        public async Task<IActionResult> ChangeTaskStatus(Guid taskId, string newStatus, DateTime? newDate)
        {
            try
            {
                await reportService.ChangeTaskStatus(taskId, newStatus, newDate);
            }
            catch (Exception ex) when (
                 ex is NullReferenceException || ex is ArgumentNullException)
            {

                TempData[nameof(ErrorData)] = ex.Message;
            }
            return RedirectToAction(nameof(MyTasks));


        }
    }
}
