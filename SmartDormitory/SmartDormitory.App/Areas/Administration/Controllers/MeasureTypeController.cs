using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Areas.Administration.Models.MeasureType;
using SmartDormitory.App.Infrastructure.Common;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartDormitory.App.Areas.Administration.Controllers
{
	[Area(WebConstants.AdministrationArea)]
	[Authorize(Policy = WebConstants.AdminPolicy)]
	public class MeasureTypeController : Controller
	{
		private readonly IMeasureTypeService measureTypeService;

		public MeasureTypeController(IMeasureTypeService measureTypeService)
		{
			this.measureTypeService = measureTypeService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var measureTypes = await measureTypeService.GetAllDeleted();
			var model = new MeasureTypesListViewModel()
			{
				MeasureTypes = measureTypes
			};

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var measureTypes = await this.measureTypeService.GetAllDeleted();

			var model = new CreateMeasureTypeViewModel()
			{
				MeasureTypes = measureTypes
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateMeasureTypeViewModel model)
		{
			if (!this.ModelState.IsValid)
			{
				TempData["Error-Message"] = "Error while trying to create a new measure type";
				return this.RedirectToAction("Index", "Home", new { area = "Administration" });
			}
			try
			{
				await this.measureTypeService.Create(model.MeasureUnit, model.SuitableSensorType);
			}
			catch (EntityAlreadyExistsException e)
			{
				TempData["Error-Message"] = e.Message;
				return this.RedirectToAction("Create", "MeasureType", new { area = "Administration" });
			}
			this.TempData["Success-Message"] = $"You successfully registered {model.SuitableSensorType} as a sensor type!";
			return this.RedirectToAction("Create", "MeasureType", new { area = "Administration" });
		}

		[HttpPost]
		public async Task<IActionResult> Delete([FromForm]string typeId)
		{
			try
			{
				await this.measureTypeService.DeleteType(typeId);
			}
			catch (EntityDoesntExistException e)
			{
				this.TempData["Error-Message"] = e.Message;
				return this.NotFound(e.Message);
			}
			this.TempData["Success-Message"] = $"Measure type was successfully deleted!";

			return this.RedirectToAction("Index", "MeasureType", new { area = "Administration" });
		}
	}
}
