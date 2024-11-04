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
    [PrimaryKey(nameof(TechId), nameof(MaintananceTaskId))]
    public class AssignedTechnicianTask
    {
        [Required]
        [ForeignKey(nameof(TechId))]
        public Guid TechId { get; set; }
        public AppUser Technician { get; set; }
        [ForeignKey(nameof(MaintananceTaskId))]

        public Guid MaintananceTaskId { get; set; }
        public MaintananceTask MaintananceTask { get; set; }

    }
}
