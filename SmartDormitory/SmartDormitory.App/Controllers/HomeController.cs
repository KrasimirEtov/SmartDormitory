using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Data;
using SmartDormitory.App.Models;
using SmartDormitory.Data.Models;
using System;
using System.Diagnostics;

namespace SmartDormitory.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly SmartDormitoryContext context;

        public HomeController(SmartDormitoryContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
           

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            var apiSensor = new ApiSensor
            {
                Id = "f1796a28-642e-401f-8129-fd7465417061",
                MinRangeValue = 15,
                MaxRangeValue = 28,
                MinPollingInterval = 40,
                Tag = "TemperatureSensor1",
                Description = "This sensor will return values between 15 and 28",
                MeasureType = "°C",
                ApiFetchUrl = "http://telerikacademy.icb.bg/api/sensor/f1796a28-642e-401f-8129-fd7465417061"
            };

            this.context.Add(apiSensor);
            this.context.SaveChanges();

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
