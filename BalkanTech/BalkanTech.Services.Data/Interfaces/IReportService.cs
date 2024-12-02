using BalkanTech.Web.ViewModels.Report;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Services.Data.Interfaces
{
    public interface IReportService
    {
        Task ChangeTaskStatus(Guid taskId, string newStatus, DateTime? newDate, string userId, bool isManager);
        Task<TasksPerTechnicianReportViewModel> ListAssignedTasks(string userId);
    }
}
