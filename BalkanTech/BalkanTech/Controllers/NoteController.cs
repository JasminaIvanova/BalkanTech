using Microsoft.AspNetCore.Mvc;

namespace BalkanTech.Web.Controllers
{
    public class NoteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
