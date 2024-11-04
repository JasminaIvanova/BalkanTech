using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static BalkanTech.Common.ErrorMessages.Rooms;

namespace BalkanTech.Web.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomService roomService;
        public RoomController( IRoomService _roomService)
        {
            roomService = _roomService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 9)
        {
            var model = await roomService.IndexGetAllRoomsAsync(search, page, pageSize);
            ViewBag.SearchQuery = search;
            if (model.Items.Count() == 0) 
            {
                TempData[nameof(NoResultFound)] = NoResultFound;
                return this.RedirectToAction("Index", "Room");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            //TODO
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new RoomAddViewModel
            {
                RoomCategories = await roomService.LoadRoomCategoriesAsync()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(RoomAddViewModel model)
        {
            if (!ModelState.IsValid) 
            {
                model.RoomCategories = await roomService.LoadRoomCategoriesAsync();
                return View(model);
            }
            bool resultAdding = await roomService.AddRoomAsync(model);
            if (resultAdding == false)
            {
                TempData[nameof(AddRoomsErrorMessage)] = AddRoomsErrorMessage;
                return this.RedirectToAction("Index", "Room");
            }
           
            return RedirectToAction(nameof(Index));
        }

      
    }
}
