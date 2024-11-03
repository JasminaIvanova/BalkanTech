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
            var model = await this.roomService.IndexGetAllRoomsAsync();
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
                RoomCategories =  await LoadRoomCategories()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(RoomAddViewModel model)
        {
            if (!ModelState.IsValid) 
            {
                model.RoomCategories = await LoadRoomCategories();
                return View(model);
            }
            Room newRoom = new Room
            {
                RoomNumber = model.RoomNumber,
                Floor = model.Floor,
                RoomCategoryId = model.RoomCategoryId,
                isAvailable = model.isAvailable
            };
            context.Rooms.Add(newRoom);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<RoomCategoryViewModel>> LoadRoomCategories() 
        {
            return await context.RoomCategories
                .Select(g => new RoomCategoryViewModel
                {
                    Id = g.Id,
                    RoomType = g.RoomType.ToString()
                }).AsNoTracking().ToListAsync();
        }
    }
}
