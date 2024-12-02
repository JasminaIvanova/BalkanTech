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
           
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            
            try
            {
                var model = await reportService.ListAssignedTasks(userId);
                return View(model);
            }
            catch (Exception ex) when (ex is NullReferenceException || ex is ArgumentNullException || ex is ArgumentException)
            {
                TempData[nameof(ErrorData)] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
           

        }
        [HttpPost]
        public async Task<IActionResult> ChangeTaskStatus(Guid taskId, string newStatus, DateTime? newDate)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            bool isUserManager = User.IsInRole("Manager");
            try
            {
                await reportService.ChangeTaskStatus(taskId, newStatus, newDate, userId, isUserManager);
            }
            catch (Exception ex) when (
                 ex is NullReferenceException 
                 || ex is ArgumentNullException 
                 || ex is UnauthorizedAccessException 
                 || ex is ArgumentException)
            {

                TempData[nameof(ErrorData)] = ex.Message;
            }
            return RedirectToAction(nameof(MyTasks));


        }
    }
}
