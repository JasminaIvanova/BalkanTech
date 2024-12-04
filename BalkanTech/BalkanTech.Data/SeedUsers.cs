using BalkanTech.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Data
{
    public class SeedUsers
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            var roles = new[] { "Admin", "Technician" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                }
            }

            var users = new List<AppUser>{
            new AppUser
            {
                UserName = "admin@admin",
                Email = "admin@admin",
                FirstName = "admin",
                LastName = "admin",
            },
            new AppUser
            {
                UserName = "tech1@tech",
                Email = "tech1@tech",
                FirstName = "Zhorzh",
                LastName = "Ivanov",
            },
            new AppUser
            {
                UserName = "tech2@tech",
                Email = "tech2@tech",
                FirstName = "Jasmina",
                LastName = "Ivanova",
            },
             new AppUser
            {
                UserName = "tech3@tech",
                Email = "tech3@tech",
                FirstName = "Kristiana",
                LastName = "Ivanova",
            }
             };

            foreach (var user in users)
            {
                var existingUser = await userManager.FindByIdAsync(user.Id.ToString());
                if (existingUser == null)
                {
                    var createResult = await userManager.CreateAsync(user, "12345678"); 
                    if (createResult.Succeeded)
                    {
                        var role = user.Email == "admin@admin" ? "Admin" : "Technician";
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }
        }
    }
}
