using Microsoft.AspNetCore.Mvc;

namespace BalkanTech.Web.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
