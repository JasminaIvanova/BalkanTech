using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Web.ViewModels.Task
{
    public class TaskDeleteViewModel
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public int RoomNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
