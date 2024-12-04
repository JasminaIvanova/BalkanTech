using BalkanTech.Data.Models;
using BalkanTech.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Services.Data.Interfaces
{
    public interface IAdminService
    {
        Task<List<TechsIndexViewModel>> IndexGetAllTechsAsync();
        Task<IdentityResult> ChangeRoleOfUserAsync(Guid userId, string role);
        Task<AppUser> FindAppUserByIdAsync(Guid userId);

        Task<IdentityResult> DeleteUserAsync(Guid userId);
        Task AddRoomCategoryAsycn(CategoryIndexViewModel model);
        Task<ManageCategoriesViewModel> ListRoomCategoriesAsync();
        Task DeleteRoomCategoryAsync(Guid categoryId);

        Task AddTaskCategoryAsycn(CategoryIndexViewModel model);
        Task<ManageCategoriesViewModel> ListTaskCategoriesAsync();
        Task DeleteTaskCategoryAsync(Guid categoryId);
    }
}
