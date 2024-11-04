using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BalkanTech.Common.Constants;

namespace BalkanTech.Data.Models
{
    public class TaskCategory
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MinLength(MinValueCategoryNameLength)]
        [MaxLength(MaxValueCategoryNameLength)]
        public string Name { get; set; } = string.Empty;
        public ICollection<MaintananceTask> MaintananceTasks { get; set; } = new List<MaintananceTask>();
    }
}
