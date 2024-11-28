using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BalkanTech.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    //TODO validations, checkes for nulls etc
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
            await adminService.ChangeRoleOfUserAsync(userId, role);
            return RedirectToAction(nameof(Index));
        }

    }
}
