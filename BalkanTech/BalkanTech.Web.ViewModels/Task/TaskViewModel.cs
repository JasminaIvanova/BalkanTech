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
        public int RoomNumber { get; set; }
        public List<MaintananceTask> ToBeCompletedTasks { get; set; } = new List<MaintananceTask>();
        public List<MaintananceTask> CompletedTasks { get; set; } = new List<MaintananceTask>();
        public List<string> Categories { get; set; } = new List<string>();
    }
}
