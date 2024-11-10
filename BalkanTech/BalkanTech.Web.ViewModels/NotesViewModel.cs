using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BalkanTech.Common.Constants;

namespace BalkanTech.Web.ViewModels
{
    public class NotesViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(MaxValueNoteLength)]
        [MinLength(MinValueNoteLength)]
        public string NoteComment { get; set; } = string.Empty;

        public DateTime NoteDate { get; set; }

        public string AppUserName { get; set; } = string.Empty;
    }
}
