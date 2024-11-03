using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BalkanTech.Web.Controllers
{
    public class RoomController : Controller
    {
        private readonly BalkanDbContext context;
        private readonly IRoomService roomService;
        public RoomController(BalkanDbContext _context, IRoomService _roomService)
        {
            context = _context;
            roomService = _roomService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await roomService.IndexGetAllRoomsAsync();
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
            await roomService.AddRoomAsync(model);
            return RedirectToAction(nameof(Index));
        }

      
    }
}
