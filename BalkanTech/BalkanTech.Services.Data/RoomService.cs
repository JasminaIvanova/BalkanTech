using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BalkanTech.Services.Data
{
    public class RoomService : IRoomService
    {
        private readonly BalkanDbContext context;
        public RoomService(BalkanDbContext _context)
        {
            context = _context;
        }

        public async Task<IEnumerable<RoomCategoryViewModel>> LoadRoomCategoriesAsync()
        {
            return await context.RoomCategories
                .Select(g => new RoomCategoryViewModel
                {
                    Id = g.Id,
                    RoomType = g.RoomType.ToString()
                }).AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<RoomsIndexViewModel>> IndexGetAllRoomsAsync() 
        {
            return await context.Rooms
                .Select(r => new RoomsIndexViewModel
                {
                    RoomNumber = r.RoomNumber,
                    Floor = r.Floor,
                    isAvailable = r.isAvailable ? "Available" : "Not Available",
                    RoomCategory = r.RoomCategory.RoomType.ToString()
                }).ToListAsync();
        }

        public async Task AddRoomAsync(RoomAddViewModel model)
        {
            Room newRoom = new Room
            {
                RoomNumber = model.RoomNumber,
                Floor = model.Floor,
                RoomCategoryId = model.RoomCategoryId,
                isAvailable = model.isAvailable
            };
            context.Rooms.Add(newRoom);
            await context.SaveChangesAsync();
        }
     
    }
}
