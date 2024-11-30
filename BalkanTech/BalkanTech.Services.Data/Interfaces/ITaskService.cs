using BalkanTech.Data.Models;
using BalkanTech.Web.ViewModels.Task;
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
        Task<TaskViewModel> IndexGetAllTasksAsync(Guid roomId, int roomNumber,  int? completedPage, int? toBeCompletedPage, int pageSize, string category = "All");
        Task<TaskAddViewModel> LoadTaskAddModel(Guid roomId);
        
        Task AddTaskAsync(TaskAddViewModel model, DateTime parsedDate);
        Task<IEnumerable<TaskTechnicianViewModel>> LoadTechniciansAsync();
        Task<IEnumerable<TaskAddRoomViewModel>> LoadRoomsAsync();

        Task<IEnumerable<TaskCategoryViewModel>> LoadTaskCategoriesAsync();
        Task<TaskDetailsViewModel> LoadTaskDetailsAsync(Guid id);

        Task<MaintananceTask> LoadMaintananceTaskAsync(Guid id);
        Task<TaskAddViewModel> LoadEditTaskAsync(Guid Id);
        Task EditTaskAsync(TaskAddViewModel model, DateTime parsedDate);
        Task<int> GetRoomNumberByIdAsync(Guid roomId);

        Task<TaskDeleteViewModel> LoadDeleteViewModelAsync(Guid id);
        Task DeleteTaskAsync(TaskDeleteViewModel model);
    }
}
