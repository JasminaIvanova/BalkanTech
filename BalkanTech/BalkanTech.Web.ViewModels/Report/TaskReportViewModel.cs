using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Web.ViewModels.Report
{
    public class TaskReportViewModel

    {
        public Guid UserId { get; set; }
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public string TaskCategory { get; set; }
        public string Name { get; set; }
    }
}
