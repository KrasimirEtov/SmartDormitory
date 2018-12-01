using AlphaCinemaServices.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartDormitory.App.Models.Sensor;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using SmartDormitory.Services.Models.IcbSensors;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartDormitory.App.Controllers
{
	public class SensorController : Controller
	{
		private readonly ISensorsService sensorsService;
		private readonly IIcbSensorsService icbSensorsService;

		public SensorController(ISensorsService sensorsService, IIcbSensorsService icbSensorsService)
		{
			this.sensorsService = sensorsService;
			this.icbSensorsService = icbSensorsService;
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> RegisterIndex()
		{
			var sensorTypes = await this.icbSensorsService.GetIcbSensorsTypes();
			var sensors = await this.icbSensorsService.GetSensorsByMeasureTypeId();

			var model = new IcbSensorTypesViewModel
			{
				MeasureTypes = new SelectList(sensorTypes, "Id", "SuitableSensorType"),
				MeasureTypeId = string.Empty,
				IcbSensors = this.MapSensorServiceModelToViewModel(sensors)
			};

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> LoadSensorsByType(string measureTypeId)
		{
			try
			{
				var sensors = await this.icbSensorsService.GetSensorsByMeasureTypeId(measureTypeId);
				var model = this.MapSensorServiceModelToViewModel(sensors);

				return PartialView("_IcbSensorsByTypeResult", model);
			}
			catch (EntityDoesntExistException e)
			{
				return NotFound(e.Message);
			}
		}

		[HttpGet]
		public IActionResult GoogleMapChooseAdress()
		{
			return View();
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Create(string sensorId)
		{
			// TODO: Better implementation
			var sensor = await this.icbSensorsService.GetSensorById(sensorId);
			var model = new CreateSensorViewModel()
			{
				IcbSensorId = sensorId,
				PollingInterval = sensor.PollingInterval
			};

			if (sensor.MeasureType.MeasureUnit == "(true/false)")
			{
				model.IsSwitch = true;
			}
			else
			{
				model.MinRangeValue = sensor.MinRangeValue;
				model.MaxRangeValue = sensor.MaxRangeValue;
			}
			return View(model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public IActionResult Create(CreateSensorViewModel model)
		{
			if (!this.ModelState.IsValid)
			{
				throw new InvalidClientInputException("Register new sensor failed");
			}

			this.sensorsService.RegisterNewSensor(model.OwnerId, model.IcbSensorId, model.Name, model.Description,
				model.PollingInterval, model.IsPublic, model.AlarmOn, model.MinRangeValue, model.MaxRangeValue,
				model.Longtitude, model.Latitude);

			this.TempData["Success-Message"] = $"You successfully registered a new sensor!";
			return this.RedirectToAction("Create", "Sensor", new { sensorId = model.IcbSensorId });
		}

		private List<IcbSensorsListViewModel> MapSensorServiceModelToViewModel(
			IEnumerable<IcbSensorRegisterListServiceModel> sensors)
		{
			return sensors.Select(s => new IcbSensorsListViewModel
			{
				Id = s.Id,
				Description = s.Description,
				PollingInterval = "Minimum refresh time: " + s.PollingInterval,
				Tag = this.ExtractTag(s.Tag),
				//set image url depends on tag
			}).ToList();
		}

		private string ExtractTag(string source)
		{
			var results = Regex.Split(source, @"(?<!^)(?=[A-Z0-9])");

			return string.Join(" ", results);
		}
	}
}
