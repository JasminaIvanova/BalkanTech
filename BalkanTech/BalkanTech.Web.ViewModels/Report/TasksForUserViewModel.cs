using BalkanTech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Web.ViewModels.Report
{
    public class TasksForUserViewModel
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public List<TaskReportViewModel> Tasks { get; set; } = new List<TaskReportViewModel>();
    }
}
