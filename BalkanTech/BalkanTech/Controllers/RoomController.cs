using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BalkanTech.Web.Controllers
{
    public class RoomController : Controller
    {
        private readonly BalkanDbContext context;
        public RoomController(BalkanDbContext _context)
        {
            context = _context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await context.Rooms
                .Select(r => new RoomsIndexViewModel
                {
                    RoomNumber = r.RoomNumber,
                    Floor = r.Floor,
                    isAvailable = r.isAvailable ? "Available" : "Not Available",
                    RoomCategory = r.RoomCategory.RoomType.ToString()
                }).ToListAsync();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            return View();
        }
     }
}
