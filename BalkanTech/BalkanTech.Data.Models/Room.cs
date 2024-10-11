using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BalkanTech.Common.Constants;

namespace BalkanTech.Data.Models
{
    public class Room
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [Range(MinValueRoomNumber, MaxValueRoomNumber)]
        public int RoomNumber { get; set; }
        [Required]
        [Range(MinFloorRoomNumber, MaxFloorRoomNumber)]
        public int Floor { get; set; }
        [Required]
        [ForeignKey(nameof(RoomCategoryId))]
        public Guid RoomCategoryId { get; set; }
        public RoomCategory RoomCategory { get; set; } = null!;
        [Required]
        public bool isAvailable { get; set; }
    }
}
