using BalkanTech.Data.Models;
using BalkanTech.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Services.Data.Interfaces
{
    public interface ITaskService
    {
        Task<TaskViewModel> IndexGetAllTasksAsync(int roomNumber, string category = "All");
        
        Task AddTaskAsync(TaskAddViewModel model, DateTime parsedDate);
        Task<IEnumerable<TaskTechnicianViewModel>> LoadTechniciansAsync();
        Task<IEnumerable<TaskAddRoomViewModel>> LoadRoomsAsync();

        Task<IEnumerable<TaskCategoryViewModel>> LoadTaskCategoriesAsync();

        Task<JsonResult> ChangeTaskStatus(Guid id, string newStatus, DateTime? newDate);

        Task<TaskDetailsViewModel> LoadTaskDetailsAsync(Guid id);
    }
}
