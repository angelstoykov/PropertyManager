using Microsoft.AspNetCore.Mvc;
using PropertyManager.Data.Models.Entities;
using PropertyManager.Data.Models.Enums;
using PropertyManager.WEB.Models;
using System.Diagnostics;

namespace PropertyManager.WEB.Controllers
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
            var testList = new List<Property>();
            
            return View(testList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
