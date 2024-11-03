using BalkanTech.Data;
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


    }
}
