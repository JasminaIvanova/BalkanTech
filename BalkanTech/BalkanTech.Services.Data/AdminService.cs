using BalkanTech.Data.Models;
using BalkanTech.Data;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BalkanTech.Services.Data
{
    //TODO validations, checkes for nulls etc
    public class AdminService : IAdminService
    {
        private readonly BalkanDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;

        public AdminService(BalkanDbContext _context, UserManager<AppUser> _userManager, RoleManager<IdentityRole<Guid>> _roleManager)
        {
            context = _context;
            userManager = _userManager;
            roleManager = _roleManager;
        }
        public async Task<List<TechsIndexViewModel>> IndexGetAllTechsAsync()
        {
            var adminId = (await userManager.GetUsersInRoleAsync("Admin")).FirstOrDefault()?.Id;
            var allUsersExceptAdmin = await userManager.Users.Where(u => u.Id != adminId).ToListAsync();
            var techsModel = new List<TechsIndexViewModel>();

            foreach (var user in allUsersExceptAdmin)
            {
                var roleName = (await userManager.IsInRoleAsync(user, "Technician")) ? "Technician" :
                               (await userManager.IsInRoleAsync(user, "Manager")) ? "Manager" :
                               "Unknown";

                techsModel.Add(new TechsIndexViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleName = roleName,
                    Email = user.Email ?? string.Empty,
                });
            }
            return techsModel;
        }
        public async Task ChangeRoleOfUserAsync(Guid userId, string role)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            var currentUserRole = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentUserRole);
            await userManager.AddToRoleAsync(user, role);
        }

    }
}
