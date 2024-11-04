using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Data.Models
{
    [PrimaryKey(nameof(AppUserId), nameof(MaintananceTaskId))]
    public class AssignedTechnicianTask
    {
        [Required]
        public Guid AppUserId { get; set; }
        [ForeignKey(nameof(AppUserId))]
        public AppUser AppUser { get; set; } = null!;
        public Guid MaintananceTaskId { get; set; }
        [ForeignKey(nameof(MaintananceTaskId))]
        public MaintananceTask MaintananceTask { get; set; }

    }
}
