using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;
using static BalkanTech.Common.Constants;
using static BalkanTech.Common.ErrorMessages.Rooms;
using static BalkanTech.Common.ErrorMessages;

namespace BalkanTech.Web.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITaskService taskService;
        private readonly BalkanDbContext context;
        public TaskController(BalkanDbContext _context, UserManager<AppUser> userManager, ITaskService _taskService)
        {
            context = _context;
            _userManager = userManager;
            taskService = _taskService;

        }
        [HttpGet]
        public async Task<IActionResult> Index(Guid roomId, int roomNumber, int? completedPage = 1, int? toBeCompletedPage = 1, int pageSize = 3, string category = "All")
        {
            try
            {
                var model = await taskService.IndexGetAllTasksAsync(roomId, roomNumber, completedPage,toBeCompletedPage,pageSize, category);
                ViewData["RoomId"] = roomId;
                ViewData["RoomNumber"] = roomNumber;
                ViewData["Category"] = category;
                ViewData["CompletedPage"] = completedPage ?? 1;
                ViewData["ToBeCompletedPage"] = toBeCompletedPage ?? 1;
                return View(model);
            }
            catch (Exception ex) when (ex is ArgumentException ||
                            ex is NullReferenceException)
            {

                TempData[nameof(ErrorRoomNumber)] = ex.Message;
            }
            return RedirectToAction("Index", "Room");

        }
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Add(Guid roomId)
        {

            var model = await taskService.LoadTaskAddModel(roomId);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Add(TaskAddViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model = await taskService.LoadTaskAddModel(Guid.Empty);
                    return View(model);
                }
                if (!DateTime.TryParseExact(model.DueDate, dateFormat, CultureInfo.InvariantCulture,
                   DateTimeStyles.None, out DateTime parsedDueDate))
                {
                    throw new InvalidOperationException("Invalid date format.");
                }
                await taskService.AddTaskAsync(model, parsedDueDate);
                var roomNumber = await taskService.GetRoomNumberByIdAsync(model.RoomId);
                return RedirectToAction("Index", "Task", new { roomId = model.RoomId, roomNumber });
            }
            catch (Exception ex) when (ex is InvalidOperationException ||
                             ex is NullReferenceException || ex is ArgumentNullException)
            {
                TempData[nameof(ErrorData)] = ex.Message;
                model = await taskService.LoadTaskAddModel(Guid.Empty);
                return View(model);
            }
        }

      

        [HttpGet]
        public async Task<IActionResult> TaskDetails(Guid id)
        {
            try
            {
                var model = await taskService.LoadTaskDetailsAsync(id);
                return View(model);
            }
            catch (NullReferenceException ex)
            {

                TempData[nameof(ErrorData)] = ex.Message;
                return RedirectToAction("Index", "Room");
            }

          
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var model = await taskService.LoadEditTaskAsync(id);
                return View(model);
            }
            catch (Exception ex) when (ex is NullReferenceException || ex is ArgumentException)
            {

                TempData[nameof(ErrorData)] = ex.Message;
                return RedirectToAction("Index", "Room");
            }

            
        }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(TaskAddViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model = await taskService.LoadEditTaskAsync(Guid.Empty);
                    return View(model);
                }
                if (!DateTime.TryParseExact(model.DueDate, dateFormat, CultureInfo.InvariantCulture,
           DateTimeStyles.None, out DateTime parsedDueDate))
                {
                    throw new InvalidOperationException("Invalid date format.");
                }
                await taskService.EditTaskAsync(model, parsedDueDate);

                return RedirectToAction("TaskDetails", new { id = model.Id });
            }
            catch (Exception ex) when (ex is InvalidOperationException ||
                              ex is NullReferenceException || ex is ArgumentNullException)
            {
                TempData[nameof(ErrorData)] = ex.Message;
                model = await taskService.LoadEditTaskAsync(model.Id);
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var model = await taskService.LoadDeleteViewModelAsync(id);
                return PartialView("_DeletePartial", model);
            }
            catch (NullReferenceException ex)
            {

                TempData[nameof(ErrorData)] = ex.Message;
                return RedirectToAction("Index", "Room");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(TaskDeleteViewModel model)
        {
            try
            {
                var result = await taskService.DeleteTaskAsync(model);
                if (result == true)
                {
                    TempData[nameof(SuccessData)] = SuccessfullyDeletedTask;
                }
                else
                {
                    TempData[nameof(ErrorData)] = ErrorDeleteTask;
                }
                
            }
            catch (NullReferenceException ex)
            {
                TempData[nameof(ErrorData)] = ex.Message;
            }
            return RedirectToAction("Index", "Task", new { roomId = model.RoomId, model.RoomNumber });

        }
    }
}
