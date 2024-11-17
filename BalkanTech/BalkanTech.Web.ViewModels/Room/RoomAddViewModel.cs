using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BalkanTech.Common.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BalkanTech.Web.ViewModels.Room
{
    public class RoomAddViewModel
    {
        [Required]
        [Range(MinValueRoomNumber, MaxValueRoomNumber)]
        public int RoomNumber { get; set; }
        [Required]
        [Range(MinFloorRoomNumber, MaxFloorRoomNumber)]
        public int Floor { get; set; }
        [Required]

        public Guid RoomCategoryId { get; set; }
        public IEnumerable<RoomCategoryViewModel>? RoomCategories { get; set; }
        [Required]
        public bool isAvailable { get; set; }

    }
}
