using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Areas.Administration.Models.Home;
using SmartDormitory.Services.Contracts;
using System.Threading.Tasks;

namespace SmartDormitory.App.Areas.Administration.Controllers
{
	[Area("Administration")]
	[Authorize(Policy = "Admin")]
	public class HomeController : Controller
	{
		private readonly IUserService userService;
		private readonly ISensorsService sensorsService;

		public HomeController(IUserService userService, ISensorsService sensorsService)
		{
			this.userService = userService;
			this.sensorsService = sensorsService;
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var users = await userService.TotalUsers();
			var sensors = await sensorsService.TotalSensors();

			var model = new DashboardViewModel()
			{
				UsersCount = users,
				SensorsCount = sensors
			};

			return View(model);
		}
	}
}
