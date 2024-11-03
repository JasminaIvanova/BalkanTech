using BalkanTech.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Services.Data.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomsIndexViewModel>> IndexGetAllRoomsAsync();

    }
}
