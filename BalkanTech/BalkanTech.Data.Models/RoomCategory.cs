using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BalkanTech.Common.Constants;

namespace BalkanTech.Data.Models
{
    public class RoomCategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(MinRoomTypeLength)]
        [MaxLength(MaxRoomTypeLength)]

        public string RoomType { get; set; } = string.Empty;
        public IEnumerable<Room> Rooms { get; set; } = new List<Room>();
    }
}
