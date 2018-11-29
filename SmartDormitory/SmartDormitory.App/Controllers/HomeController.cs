using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Models;
using SmartDormitory.Services.Contracts;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SmartDormitory.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISensorsService sensorsService;

        public HomeController(ISensorsService sensorsService)
        {
            this.sensorsService = sensorsService;
        }

        public IActionResult Index()
        {
            // seed some fake sensonrs
            //this.sensorsService.SeedSomeSensorsForMaps();
            //var sensorsCoordinates = await this.sensorsService.GetAllPublicSensorsCoordinates();
            
            return View();
        }
        
        [HttpGet]
        public async Task<JsonResult> GetSensorsCoordinates()
        {
            // seed some fake sensonrs
            //this.sensorsService.SeedSomeSensorsForMaps();

            var sensorsCoordinates = await this.sensorsService.GetAllPublicSensorsCoordinates();

            return this.Json(sensorsCoordinates);
            //return View(sensorsCoordinates);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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
