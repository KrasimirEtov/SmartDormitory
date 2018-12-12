using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Infrastructure.Extensions;
using SmartDormitory.App.Models;
using SmartDormitory.App.Models.Home;
using SmartDormitory.Services.Contracts;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISensorsService sensorsService;
        private readonly IMeasureTypeService measureTypeService;
        private readonly IIcbSensorsService icbSensorsService;
        private readonly IUserService userService;

        public HomeController(ISensorsService sensorsService, IMeasureTypeService measureTypeService,
            IIcbSensorsService icbSensorsService, IUserService userService)
        {
            this.sensorsService = sensorsService;
            this.measureTypeService = measureTypeService;
            this.icbSensorsService = icbSensorsService;
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var sensorTypesCount = await this.measureTypeService.TotalCount();
            var sensorModelsCount = await this.icbSensorsService.TotalCount();
            var registeredSensorsCount = await this.sensorsService.TotalSensors();
            var usersCount = await this.userService.TotalUsers();

            var model = new HomeIndexViewModel
            {
                TotalUsers = usersCount,
                SensorTypesCount = sensorTypesCount,
                SensorModelsCount = sensorModelsCount,
                RegisteredSensorsCount = registeredSensorsCount
            };

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetSensorsCoordinates()
        {
            var data = await this.sensorsService.GetAllPublicCoordinates();

            if (this.User.Identity.IsAuthenticated)
            {
                var userPrivateSensors = await sensorsService
                                                    .GetAllUserPrivateCoordinates(User.GetId());

                data = data.Concat(userPrivateSensors);
            }

            return this.Json(data);
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
