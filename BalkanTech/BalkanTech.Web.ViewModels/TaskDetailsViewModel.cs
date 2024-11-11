using BalkanTech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Web.ViewModels
{
    public class TaskDetailsViewModel
    {
        public Guid Id { get; set; }
        public required string Name { get; set; } 
        public required string Description { get; set; } 

        public int RoomNumber { get; set; }
        public required DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public required string Status { get; set; } 
        public required string TaskCategory { get; set; } 
        
        public List<TaskTechnicianViewModel>? AssignedTechnicians { get; set; } = new List<TaskTechnicianViewModel>();
        public List<NotesViewModel>? Notes { get; set; } = new List<NotesViewModel>();
    }
}
