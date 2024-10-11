using Microsoft.AspNetCore.Mvc;

namespace BalkanTech.Web.Controllers
{
    public class RoomControler : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
