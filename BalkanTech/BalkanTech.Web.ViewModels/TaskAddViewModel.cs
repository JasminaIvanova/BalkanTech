using BalkanTech.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BalkanTech.Common.Constants;

namespace BalkanTech.Web.ViewModels
{
    public class TaskAddViewModel
    {
        [Required]
        [MaxLength(MaxValueDescriptionLength)]
        [MinLength(MinValueDescriptionLength)]
        public string Description { get; set; } = string.Empty;
        public Guid RoomId { get; set; }
        public IEnumerable<int>? RoomNumbers { get; set; }
        [Required]
        public Guid TaskCategoryId { get; set; }
        public IEnumerable<string>? TaskCategories { get; set; } 

        [Required]
        public string DueDate { get; set; }
        public string? CompletedDate { get; set; }
        public IEnumerable<string> AssignedTechnicians{ get; set; }
    }
}
