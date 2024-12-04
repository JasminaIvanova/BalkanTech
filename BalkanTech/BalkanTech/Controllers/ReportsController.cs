using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Report;
using BalkanTech.Web.ViewModels.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Globalization;
using System.Security.Claims;
using static BalkanTech.Common.ErrorMessages.Tasks;
using static BalkanTech.Common.Constants;

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
        public async Task<IActionResult> TechTasks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            try
            {
                var model = await reportService.GetTasksForUserAsync(userId);
                return View(model);
            }
            catch (Exception ex) when (ex is NullReferenceException || ex is ArgumentNullException || ex is ArgumentException)
            {
                TempData[nameof(ErrorData)] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangeTaskStatus(Guid taskId, string newStatus, DateTime? newDate, string? userId)
        {
            bool isUserManager = User.IsInRole("Manager");

            if (!isUserManager)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            }

            try
            {
                await reportService.ChangeTaskStatus(taskId, newStatus, newDate, userId, isUserManager);
                TempData[nameof(SuccessData)] = "Task status successfully changed";
            }
            catch (Exception ex) when (
                 ex is NullReferenceException
                 || ex is ArgumentNullException
                 || ex is UnauthorizedAccessException
                 || ex is ArgumentException
                 || ex is InvalidOperationException)
            {
                TempData[nameof(ErrorData)] = ex.Message;
            }

            if (isUserManager)
            {
                return RedirectToAction(nameof(UserTasks), new { userId = userId });
            }

            return RedirectToAction(nameof(TechTasks));
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ChooseReportTech()
        {
            var users = await reportService.GetAllTechUsersAsync();
            return View(new ManagerReportViewModel { Users = users });
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UserTasks(string userId)
        { 
           try
            {
                var tasks = await reportService.GetTasksForUserAsync(userId);
                return View(nameof(TechTasks), tasks);

            }
            catch (Exception ex) when (
                 ex is NullReferenceException
                 || ex is ArgumentNullException
                 || ex is ArgumentException)
            {

                TempData[nameof(ErrorData)] = ex.Message;
                return RedirectToAction(nameof(ChooseReportTech));
            }

           

         

          
        }


    }
}
