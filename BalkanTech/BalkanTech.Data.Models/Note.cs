using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BalkanTech.Common.Constants;

namespace BalkanTech.Data.Models
{
    public class Note
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(MaxValueNoteLength)]
        [MinLength(MinValueNoteLength)]
        public string NoteComment { get; set; } = string.Empty;
        [Required]
        [ForeignKey(nameof(MaintananceTaskId))]
        public Guid MaintananceTaskId { get; set; }
        public MaintananceTask MaintananceTask { get; set; } = null!;
        [Required]
        [ForeignKey(nameof(AppUserId))]
        public Guid AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;
        public DateTime NoteDate { get; set; } = DateTime.Now;
    }
}
