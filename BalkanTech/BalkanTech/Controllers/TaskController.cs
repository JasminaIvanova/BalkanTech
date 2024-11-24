﻿using BalkanTech.Data;
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

namespace BalkanTech.Web.Controllers
{
    //TODO -> validations , redirect after adding task, validation for correct task, pages for tasks
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
        public async Task<IActionResult> Index(Guid roomId, int roomNumber, string category = "All")
        {
            try
            {
                var model = await taskService.IndexGetAllTasksAsync(roomId, roomNumber, category);
                if (model == null)
                {
                    return NotFound("Room not found.");
                }
                return View(model);
            }
            catch (InvalidOperationException roomEx)
            {

                TempData[nameof(ErrorRoomNumber)] = ErrorRoomNumber;
                return RedirectToAction("Index", "Room");
            }

        }
        [HttpGet]
        public async Task<IActionResult> Add(Guid roomId)
        {
            var model = await taskService.LoadTaskAddModel(roomId);


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TaskAddViewModel model)
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

        [HttpPost]
        public async Task<IActionResult> ChangeTaskStatus(Guid id, string newStatus, DateTime? newDate)
        {
            return await taskService.ChangeTaskStatus(id, newStatus, newDate);
        }

        [HttpGet]
        public async Task<IActionResult> TaskDetails(Guid id)
        {
            var model = await taskService.LoadTaskDetailsAsync(id);
            if (model == null)
            {
                return NotFound("The task with the specified ID does not exist.");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {

            var model = await taskService.LoadEditTaskAsync(id);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(TaskAddViewModel model)
        {

            if (!DateTime.TryParseExact(model.DueDate, dateFormat, CultureInfo.InvariantCulture,
              DateTimeStyles.None, out DateTime parsedDueDate))
            {
                throw new InvalidOperationException("Invalid date format.");
            }
            await taskService.EditTaskAsync(model, parsedDueDate);

            return RedirectToAction("TaskDetails", new { id = model.Id });

        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var task = await context.MaintananceTasks.Include(t =>t.Room).FirstOrDefaultAsync(t => t.Id == id);
            var model = new TaskDeleteViewModel
            {

                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                RoomNumber = task.Room.RoomNumber,
                RoomId = task.RoomId,
            };
            return PartialView("_DeletePartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(TaskDeleteViewModel model)
        {
            var task = await context.MaintananceTasks.Include(t => t.Room).FirstOrDefaultAsync(t => t.Id == model.Id);
            task.IsDeleted = true;
            await context.SaveChangesAsync();
            return RedirectToAction("Index", "Task", new { roomId = task.RoomId, task.Room.RoomNumber });
        }
    }
}
