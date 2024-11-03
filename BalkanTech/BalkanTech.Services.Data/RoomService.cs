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
        public async Task<PagedResult<RoomsIndexViewModel>> IndexGetAllRoomsAsync(int page, int pageSize)
        {
            var query = context.Rooms
       .Select(r => new RoomsIndexViewModel
       {
           RoomNumber = r.RoomNumber,
           Floor = r.Floor,
           isAvailable = r.isAvailable ? "Available" : "Not Available",
           RoomCategory = r.RoomCategory.RoomType.ToString()
       });

            // Count total items before pagination
            var totalItems = await query.CountAsync();

            // Paginate the results
            var rooms = await query
                .Skip((page - 1) * pageSize) // Calculate the starting point based on the current page
                .Take(pageSize)               // Limit to the specified number of items
                .ToListAsync();

            // Return the paginated result
            return new PagedResult<RoomsIndexViewModel>
            {
                Items = rooms,
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };
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
