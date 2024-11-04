using BalkanTech.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BalkanTech.Web.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index(Guid id, int roomNumber)
        {
            var model = new TaskViewModel();
            model.RoomNumber = roomNumber; 
            return View(model);
        }
    }
}
