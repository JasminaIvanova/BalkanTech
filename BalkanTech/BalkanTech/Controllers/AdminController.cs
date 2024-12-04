using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BalkanTech.Common.ErrorMessages.Admin;
using static BalkanTech.Common.ErrorMessages;

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
            return View(); 
        }
        [HttpGet]
        public async Task<IActionResult> ManageUsers()
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

            return RedirectToAction(nameof(ManageUsers));
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

        [HttpGet]
        public async Task<IActionResult> ManageRoomCategories()
        {
            var model = await adminService.ListRoomCategoriesAsync();
            return View("ManageCategories", model);
        }
        [HttpPost]
        public async Task<IActionResult> AddRoomCategory(CategoryIndexViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[nameof(ErrorData)] = "Invalid input.";
                    return RedirectToAction(nameof(ManageRoomCategories));
                }
                await adminService.AddRoomCategoryAsycn(model);
                TempData[nameof(SuccessData)] = "Room category added successfully.";
                
            }
            catch (Exception ex) when (ex is NullReferenceException)
            {

                TempData[nameof(ErrorData)] = ex.Message;
            }
            return RedirectToAction(nameof(ManageRoomCategories));

        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoomCategory(Guid id)
        {
            try
            {
                await adminService.DeleteRoomCategoryAsync(id);
                TempData[nameof(SuccessData)] = "Room category deleted successfully.";

            }
            catch(Exception ex) when(
               ex is NullReferenceException)
            {
                TempData[nameof(ErrorData)] = ex.Message;
            }

            return RedirectToAction(nameof(ManageRoomCategories));
        }

        [HttpGet]
        public async Task<IActionResult> ManageTaskCategories()
        {
            var model = await adminService.ListTaskCategoriesAsync();
            return View("ManageCategories", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddTaskCategory(CategoryIndexViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[nameof(ErrorData)] = "Invalid input.";
                    return RedirectToAction(nameof(ManageTaskCategories));
                }
                await adminService.AddTaskCategoryAsycn(model);
                TempData[nameof(SuccessData)] = "Task category added successfully.";

            }
            catch (Exception ex) when (ex is NullReferenceException)
            {

                TempData[nameof(ErrorData)] = ex.Message;
            }
            return RedirectToAction(nameof(ManageTaskCategories));

        }

        [HttpPost]
        public async Task<IActionResult> DeleteTaskCategory(Guid id)
        {
            try
            {
                await adminService.DeleteTaskCategoryAsync(id);
                TempData[nameof(SuccessData)] = "Task category deleted successfully.";

            }
            catch (Exception ex) when (
               ex is NullReferenceException)
            {
                TempData[nameof(ErrorData)] = ex.Message;
            }

            return RedirectToAction(nameof(ManageTaskCategories));
        }

    }
}
