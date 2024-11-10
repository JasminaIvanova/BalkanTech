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
        [MaxLength(MaxValueNameLength)]
        [MinLength(MinValueNameLength)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(MaxValueDescriptionLength)]
        [MinLength(MinValueDescriptionLength)]
        public string Description { get; set; } = string.Empty;
        public Guid RoomId { get; set; }
        public IEnumerable<TaskAddRoomViewModel>? RoomNumbers { get; set; }
        [Required]
        public Guid TaskCategoryId { get; set; }
        public IEnumerable<TaskCategoryViewModel>? TaskCategories { get; set; }

        [Required]
        public string DueDate { get; set; } = string.Empty;
        public IEnumerable<TaskAddTechnicianViewModel> Technicians { get; set; } = new List<TaskAddTechnicianViewModel>();

        public List<Guid> AssignedTechniciansIDs { get; set; } = new();
    }
}
