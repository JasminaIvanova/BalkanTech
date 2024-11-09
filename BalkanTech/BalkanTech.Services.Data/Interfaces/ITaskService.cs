using BalkanTech.Data.Models;
using BalkanTech.Web.ViewModels;
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
        Task<IEnumerable<TaskAddTechnicianViewModel>> LoadTechniciansAsync();
        Task<IEnumerable<TaskAddRoomViewModel>> LoadRoomsAsync();

        Task<IEnumerable<TaskCategoryViewModel>> LoadTaskCategoriesAsync();
    }
}
