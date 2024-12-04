using BalkanTech.Web.ViewModels.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Web.ViewModels.Report
{
    public class ManagerReportViewModel
    {
        public List<TasksPerTechnicianReportViewModel> UserTasks { get; set; } = new List<TasksPerTechnicianReportViewModel>();
        public IEnumerable<TaskTechnicianViewModel> Users { get; set; } = new List<TaskTechnicianViewModel>();
    }
}
