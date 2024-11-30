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
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BalkanTech.Services.Data
{
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
        public async Task<AppUser> FindAppUserByIdAsync(Guid userId)
        {
            if (string.IsNullOrWhiteSpace(userId.ToString())) 
            {
                throw new NullReferenceException("User ID cannot be empty");
            }
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null) 
            {
                throw new KeyNotFoundException($"User could not be found.");

            }
            return user;
        }
        public async Task<IdentityResult> ChangeRoleOfUserAsync(Guid userId, string role)
        {
            if (string.IsNullOrEmpty(role)) 
            {
                throw new NullReferenceException("Role cannot be null or empty");
            }
            var user = await FindAppUserByIdAsync(userId);
            if (!await roleManager.RoleExistsAsync(role)) 
            {
                throw new InvalidOperationException($"The role does not exist.");
            }
            var currentUserRole = await userManager.GetRolesAsync(user);
            if (currentUserRole == null) 
            {
                throw new InvalidOperationException($"No roles assigned to this user.");
            }
            await userManager.RemoveFromRolesAsync(user, currentUserRole);
            var result = await userManager.AddToRoleAsync(user, role);
            return result;

        }

        public async Task<IdentityResult> DeleteUserAsync(Guid userId)
        {
            var user = await FindAppUserByIdAsync(userId);
            var result = await userManager.DeleteAsync(user);
            return result;
        }

       
    }
}
