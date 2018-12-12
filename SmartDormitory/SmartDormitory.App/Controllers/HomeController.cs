using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SmartDormitory.App.Infrastructure.Extensions;
using SmartDormitory.App.Models;
using SmartDormitory.App.Models.Home;
using SmartDormitory.Services.Contracts;
using System;
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
        private readonly IMemoryCache memoryCache;

        public HomeController(ISensorsService sensorsService, IMeasureTypeService measureTypeService,
            IIcbSensorsService icbSensorsService, IUserService userService, IMemoryCache memoryCache)
        {
            this.sensorsService = sensorsService;
            this.measureTypeService = measureTypeService;
            this.icbSensorsService = icbSensorsService;
            this.userService = userService;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index()
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            if (!this.memoryCache.TryGetValue("SensorTypesCount", out int sensorTypesCount))
            {
                sensorTypesCount = await this.measureTypeService.TotalCount();
                this.memoryCache.Set("SensorTypesCount", sensorTypesCount, cacheOptions);
            }

            if (!this.memoryCache.TryGetValue("SensorModelsCount", out int sensorModelsCount))
            {
                sensorModelsCount = await this.icbSensorsService.TotalCount();
                this.memoryCache.Set("SensorModelsCount", sensorModelsCount, cacheOptions);
            }

            if (!this.memoryCache.TryGetValue("RegisteredSensorsCount", out int registeredSensorsCount))
            {
                registeredSensorsCount = await this.sensorsService.TotalSensors();
                this.memoryCache.Set("RegisteredSensorsCount", registeredSensorsCount, cacheOptions);
            }

            if (!this.memoryCache.TryGetValue("UsersCount", out int usersCount))
            {
                usersCount = await this.userService.TotalUsers();
                this.memoryCache.Set("UsersCount", usersCount, cacheOptions);
            }

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
