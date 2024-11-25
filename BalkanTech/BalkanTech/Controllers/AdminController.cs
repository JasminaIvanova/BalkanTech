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
        private readonly BalkanDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly IAdminService adminService;

        public AdminController(BalkanDbContext _context, UserManager<AppUser> _userManager, RoleManager<IdentityRole<Guid>> _roleManager, IAdminService _adminService)
        {
            context = _context;
            userManager = _userManager;
            roleManager = _roleManager;
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
