
using BalkanTech.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BalkanTech.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
           return View();
        }
        //500 Internal Server Error - testing
        public IActionResult SomeAction5()
        {
            try
            {
              
                throw new Exception("Some Exception Occurred");
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }
        public IActionResult Error(int? statusCode = null)
        {
 
            if (statusCode == 404)
            {
                return this.View("404NotFound");
            }
            else if (statusCode == 500)
            {
                return this.View("500InternalServerError");
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
