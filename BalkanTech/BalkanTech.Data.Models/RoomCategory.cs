using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BalkanTech.Data.Models.Enums;

namespace BalkanTech.Data.Models
{
    public class RoomCategory
    {
        public Guid Id { get; set; }
        public RoomType RoomType { get; set; }
        public IEnumerable<Room> Rooms { get; set; } = new List<Room>();
    }
}
