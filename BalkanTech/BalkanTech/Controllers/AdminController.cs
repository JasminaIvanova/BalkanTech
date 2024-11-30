using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BalkanTech.Common.ErrorMessages.Admin;

namespace BalkanTech.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService adminService;

        public AdminController(IAdminService _adminService)
        {
         
            adminService = _adminService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var model = await adminService.IndexGetAllTechsAsync();

            return View(model); 
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRole(Guid userId, string role)
        {
            try
            {
                var result = await adminService.ChangeRoleOfUserAsync(userId, role);
                if (result.Succeeded)
                {
                    TempData[nameof(AdminSuccess)] = ChangeRoleSuccess;
                }
                else
                {
                    TempData[nameof(AdminError)] = ErrorChangingRoleUser;
                }
            }
            catch (Exception ex) when (ex is KeyNotFoundException ||
                           ex is NullReferenceException ||
                           ex is InvalidOperationException)
            {
                TempData[nameof(AdminError)] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]

        public async Task<IActionResult> DeleteUser(Guid userId) 
        {
            try
            {
                var result = await adminService.DeleteUserAsync(userId);
                if (result.Succeeded)
                {
                    TempData[nameof(AdminSuccess)] = UserDeletedSuccess;
                }
                else
                {
                    TempData[nameof(AdminError)] = ErrorDeletingUser;
                }
            }
            catch (Exception ex) when (ex is KeyNotFoundException ||
                ex is NullReferenceException)
            {
                TempData[nameof(AdminError)] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
