using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Areas.Administration.Models.Home;
using SmartDormitory.App.Infrastructure.Common;
using SmartDormitory.Services.Contracts;
using System.Threading.Tasks;

namespace SmartDormitory.App.Areas.Administration.Controllers
{
    [Area(WebConstants.AdministrationArea)]
    [Authorize(Policy = WebConstants.AdminPolicy)]
    public class HomeController : Controller
    {
        private readonly IUserService userService;
        private readonly ISensorsService sensorsService;
		private readonly IMeasureTypeService measureTypeService;

		public HomeController(IUserService userService, ISensorsService sensorsService, IMeasureTypeService measureTypeService)
        {
            this.userService = userService;
            this.sensorsService = sensorsService;
			this.measureTypeService = measureTypeService;
		}

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await userService.TotalUsers();
            var sensors = await sensorsService.TotalSensors();
			var measureTypes = await this.measureTypeService.TotalCount();

            var model = new DashboardViewModel()
            {
                UsersCount = users,
                SensorsCount = sensors,
				MeasureTypesCount = measureTypes
			};

            return View(model);
        }
    }
}
