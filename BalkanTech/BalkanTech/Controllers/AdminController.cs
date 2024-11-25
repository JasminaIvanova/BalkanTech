﻿using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BalkanTech.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly BalkanDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;

        public AdminController(BalkanDbContext _context, UserManager<AppUser> _userManager, RoleManager<IdentityRole<Guid>> _roleManager)
        {
            context = _context;
            userManager = _userManager;
            roleManager = _roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var adminId = (await userManager.GetUsersInRoleAsync("Admin")).FirstOrDefault()?.Id;
            var allUsersExceptAdmin = await userManager.Users.Where(u => u.Id != adminId).ToListAsync();
            var techs = new List<TechsIndexViewModel>();

            foreach (var user in allUsersExceptAdmin)
            {
                var roleName = (await userManager.IsInRoleAsync(user, "Technician")) ? "Technician" :
                               (await userManager.IsInRoleAsync(user, "Manager")) ? "Manager" :
                               "Unknown";

                techs.Add(new TechsIndexViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleName = roleName,
                    Email = user.Email ?? string.Empty,
                });
            }


            return View(techs); 
        }

    }
}
