using BalkanTech.Data;
using BalkanTech.Data.Models;
using BalkanTech.Services.Data.Interfaces;
using BalkanTech.Web.ViewModels;
using BalkanTech.Web.ViewModels.Room;
using Microsoft.EntityFrameworkCore;
using static BalkanTech.Common.ErrorMessages;

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
        public async Task<IQueryable<Room>> LoadRoomsBySearch(string search)
        {
            var allRooms =  context.Rooms.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out _))
                {
                    allRooms = allRooms.Where(r => r.RoomNumber.ToString().Contains(search));
                }
                else
                {
                    allRooms = allRooms.Where(r => r.RoomCategory.RoomType.ToLower().Contains(search));
                }
            }
            return allRooms;
        }
        public async Task<PaginationIndexViewModel<RoomsIndexViewModel>> IndexGetAllRoomsAsync(string search, int page, int pageSize)
        {
            var allRooms = await LoadRoomsBySearch(search);

            var roomsPerPage = await allRooms
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RoomsIndexViewModel
                {
                    Id=r.Id,
                    RoomNumber = r.RoomNumber,
                    Floor = r.Floor,
                   isAvailable = r.isAvailable,
                    RoomCategory = r.RoomCategory.RoomType.ToString(),
                    PendingTasks = r.MaintananceTasks.Where(t => t.Status == "Pending").Where(t => t.IsDeleted == false).Count(),
                    CompletedTasks = r.MaintananceTasks.Where(t => t.Status == "Completed").Where(t => t.IsDeleted == false).Count(),
                    InProcessTasks = r.MaintananceTasks.Where(t => t.Status == "In Process").Where(t => t.IsDeleted == false).Count(),

                })
                .ToListAsync();

            var count = await allRooms.CountAsync();

            return new PaginationIndexViewModel<RoomsIndexViewModel>
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

        public async Task ChangeRoomStatus(Guid id)
        {
            var room = await context.Rooms.FindAsync(id);

            if (room != null)
            {
                room.isAvailable = !room.isAvailable; 
                await context.SaveChangesAsync();
            }
        }
    }
}
