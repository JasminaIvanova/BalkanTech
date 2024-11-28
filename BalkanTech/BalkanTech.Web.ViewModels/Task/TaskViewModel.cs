using BalkanTech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Web.ViewModels.Task
{
    public class TaskViewModel
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public int RoomNumber { get; set; }
        public PaginationIndexViewModel<MaintananceTask> ToBeCompletedTasks { get; set; } = new PaginationIndexViewModel<MaintananceTask>();
        public PaginationIndexViewModel<MaintananceTask> CompletedTasks { get; set; } = new PaginationIndexViewModel<MaintananceTask>();
        public List<string> Categories { get; set; } = new List<string>();
    }
}
