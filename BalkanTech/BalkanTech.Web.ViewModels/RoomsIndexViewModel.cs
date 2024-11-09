using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Web.ViewModels
{
    public class RoomsIndexViewModel
    {
        public int RoomNumber { get; set; }
        public int Floor { get; set; }
        public required string RoomCategory { get; set; }
        public required string isAvailable { get; set; }
    }
}
