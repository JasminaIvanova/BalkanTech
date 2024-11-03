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
        public async Task<RoomsIndexPagedModel<RoomsIndexViewModel>> IndexGetAllRoomsAsync(int page, int pageSize)
        {
            var allRooms = context.Rooms
               .Select(r => new RoomsIndexViewModel
               {
                   RoomNumber = r.RoomNumber,
                   Floor = r.Floor,
                   isAvailable = r.isAvailable ? "Available" : "Not Available",
                   RoomCategory = r.RoomCategory.RoomType.ToString()
               });
            
            var roomsPerPage = await allRooms
                .Skip((page - 1) * pageSize) 
                .Take(pageSize) 
                .ToListAsync();

            var count = await allRooms.CountAsync();

            return new RoomsIndexPagedModel<RoomsIndexViewModel>
            {
                Items = roomsPerPage,
                TotalItems = count,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)count / pageSize)
            };
        }

        public async Task<bool> AddRoomAsync(RoomAddViewModel model)
        {
            if (context.Rooms.Any(r => r.RoomNumber == model.RoomNumber)) 
            {
                return false;
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
            return true;
        }
     
    }
}
