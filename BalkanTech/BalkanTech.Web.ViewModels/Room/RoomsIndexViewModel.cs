using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Web.ViewModels.Room
{
    public class RoomsIndexViewModel
    {
        public Guid Id { get; set; }
        public int RoomNumber { get; set; }
        public int Floor { get; set; }
        public required string RoomCategory { get; set; }
        public bool isAvailable { get; set; }
        public int PendingTasks { get; set; }
        public int InProcessTasks { get; set; }
        public int CompletedTasks { get; set; }
    }
}
