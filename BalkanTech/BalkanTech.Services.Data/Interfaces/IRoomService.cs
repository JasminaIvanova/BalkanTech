using BalkanTech.Data.Models;
using BalkanTech.Web.ViewModels.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Services.Data.Interfaces
{
    public interface IRoomService
    {
        Task<RoomsIndexPagedModel<RoomsIndexViewModel>> IndexGetAllRoomsAsync(string search, int page, int pageSize);
        Task<IEnumerable<RoomCategoryViewModel>> LoadRoomCategoriesAsync();

        Task<bool> AddRoomAsync(RoomAddViewModel model);

        Task<IQueryable<Room>> LoadRoomsBySearch(string serach);

        Task ChangeRoomStatus(Guid id);
    }
}
