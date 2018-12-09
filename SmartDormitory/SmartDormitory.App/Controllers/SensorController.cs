using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartDormitory.App.Infrastructure.Extensions;
using SmartDormitory.App.Models.Sensor;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using SmartDormitory.Services.Models.IcbSensors;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.App.Controllers
{
	[Authorize]
	public class SensorController : Controller
	{
		private const int PageSize = 10;

		private readonly ISensorsService sensorsService;
		private readonly IIcbSensorsService icbSensorsService;
		private readonly IMeasureTypeService measureTypeService;

		public SensorController(ISensorsService sensorsService, IIcbSensorsService icbSensorsService, IMeasureTypeService measureTypeService)
		{
			this.sensorsService = sensorsService;
			this.icbSensorsService = icbSensorsService;
			this.measureTypeService = measureTypeService;
		}

		// user sensors
		[HttpGet]
		public async Task<IActionResult> MySensors()
		{
			var userId = this.User.GetId();
			var measureTypes = await this.measureTypeService.GetAll();

			var sensors = await this.sensorsService.GetUserSensors(userId);

			var model = new MySensorsViewModel
			{
				MeasureTypes = new SelectList(measureTypes, "Id", "SuitableSensorType"),
				//TODO use automapper?
				Sensors = sensors.Select(s => new MySensorListViewModel(s)).ToList()
			};

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> ReloadMySensorsTable(string measureTypeId = "all", string searchTerm = "", int alarmOn = -1, int privacy = -1)
		{
			try
			{
				var userId = this.User.GetId();
				var sensors = (await this.sensorsService
										 .GetUserSensors(userId, searchTerm, measureTypeId,
																alarmOn, privacy))
										.Select(s => new MySensorListViewModel(s))
										.ToList();

				return PartialView("_MySensorsTable", sensors);
			}
			catch (EntityDoesntExistException e)
			{
				return NotFound(e.Message);
			}
		}

		[HttpGet]
		public async Task<IActionResult> RegisterIndex()
		{
			var sensorTypes = await this.measureTypeService.GetAll();
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
		public async Task<IActionResult> LoadSensorsByType(string measureTypeId, int page = 1)
		{
			try
			{
				var sensors = await this.icbSensorsService
					.GetSensorsByMeasureTypeId(page, PageSize, measureTypeId);
				var model = this.MapSensorServiceModelToViewModel(sensors);

				return PartialView("_IcbSensorsByTypeResult", model);
			}
			catch (EntityDoesntExistException e)
			{
				return NotFound(e.Message);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Create(string icbSensorId, string userId = "")
		{
			// TODO: Better implementation
			var icbSensor = await this.icbSensorsService.GetSensorById(icbSensorId);
			if (icbSensor == null)
			{
				// TODO: Error
				return this.NotFound();
			}

			if (!string.IsNullOrWhiteSpace(userId))
			{
				if (!this.User.IsInRole("Administration"))
				{
					return this.Forbid();
				}
			}
			else
			{
				userId = this.User.GetId();
			}

			var model = new CreateUpdateSensorViewModel()
			{
				IcbSensorId = icbSensorId,
				PollingInterval = icbSensor.PollingInterval,
				ApiPollingInterval = icbSensor.PollingInterval,
				UserId = userId
			};

			if (icbSensor.MeasureType.MeasureUnit == "(true/false)")
			{
				// TODO: Попълване ако иска аларма да пита кога да се пуска - при false или true (отв, затв)
				model.IsSwitch = true;
			}
			else
			{
				model.MinRangeValue = icbSensor.MinRangeValue;
				model.MaxRangeValue = icbSensor.MaxRangeValue;
				model.ApiMaxRangeValue = icbSensor.MaxRangeValue;
				model.ApiMinRangeValue = icbSensor.MinRangeValue;
			}
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateUpdateSensorViewModel model)
		{
			if (!this.ModelState.IsValid)
			{
                TempData["Error-Message"] = "Oops something went wrong! Try again..";
                return this.RedirectToAction("Create", "Sensor", new
				{
					icbSensorId = model.IcbSensorId,
					userId = model.UserId
				});
			}
			//if (model.IsSwitch)
			//{

			//}
			// TODO: Add validation for model, change user id to here, not from view
			// TODO: Tests

			var createdSensorId = await this.sensorsService.RegisterNewSensor(model.UserId, model.IcbSensorId, model.Name,
				model.Description, model.PollingInterval, model.IsPublic,
				model.AlarmOn, model.MinRangeValue, model.MaxRangeValue,
				model.Longtitude, model.Latitude, model.SwitchOn);

			this.TempData["Success-Message"] = $"You successfully registered a new sensor!";
			return this.RedirectToAction("Details", "Sensor", new { sensorId = createdSensorId });
		}

		[HttpGet]
		public IActionResult GoogleMapChooseAdress()
		{
			return View();
		}

		private List<IcbSensorsListViewModel> MapSensorServiceModelToViewModel(
			IEnumerable<IcbSensorRegisterListServiceModel> sensors)
		{
			return sensors.Select(s => new IcbSensorsListViewModel
			{
				Id = s.Id,
				Description = s.Description,
				PollingInterval = s.PollingInterval,
				Tag = s.Tag.SplitTag(),
				//set image url depends on tag
			}).ToList();
		}

		[HttpGet]
		public async Task<IActionResult> Details(string sensorId)
		{
			try
			{
				var sensor = await sensorsService.GetSensorById(sensorId);

				var model = new DetailsSensorViewModel()
				{
					SensorId = sensorId,
					UserId = sensor.UserId,
					AlarmOn = sensor.AlarmOn,
					Description = sensor.Description,
					IsPublic = sensor.IsPublic,
					Latitude = sensor.Coordinates.Latitude,
					Longtitude = sensor.Coordinates.Longitude,
					MaxRangeValue = sensor.MaxRangeValue,
					MinRangeValue = sensor.MinRangeValue,
					Name = sensor.Name,
					PollingInterval = sensor.PollingInterval,
					StartValue = sensor.CurrentValue,
					MeasureUnit = sensor.IcbSensor.MeasureType.MeasureUnit,
					SwitchOn = sensor.SwitchOn
				};
				TempData["Success-Message"] = "Succesfully created a sensor with name - " + model.Name;
				return View(model);
			}
			catch (EntityDoesntExistException e)
			{
				TempData["Error-Message"] = e.Message;
				return this.NotFound();
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetGaudeData(string sensorId)
		{
			try
			{
				var data = await sensorsService.GetGaudeData(sensorId);
				return Json(data);
			}
			catch (EntityDoesntExistException e)
			{
				TempData["Error-Message"] = e.Message;
				return this.NotFound();
			}
		}

		[HttpGet]
		public async Task<IActionResult> Update(string sensorId)
		{
			// TODO: Better implementation
			try
			{
				var sensor = await this.sensorsService.GetSensorById(sensorId);
				var model = new CreateUpdateSensorViewModel()
				{
					AlarmOn = sensor.AlarmOn,
					Description = sensor.Description,
					IcbSensorId = sensor.IcbSensorId,
					IsPublic = sensor.IsPublic,
					Latitude = sensor.Coordinates.Latitude,
					Longtitude = sensor.Coordinates.Longitude,
					Name = sensor.Name,
					PollingInterval = sensor.PollingInterval,
					SensorId = sensor.Id,
					UserId = sensor.UserId,
					ApiPollingInterval = sensor.IcbSensor.PollingInterval,
					SwitchOn = sensor.SwitchOn
				};

				if (sensor.IcbSensor.MeasureType.MeasureUnit == "(true/false)")
				{
					// TODO: Попълване ако иска аларма да пита кога да се пуска - при false или true (отв, затв)
					model.IsSwitch = true;
				}
				else
				{
					model.MinRangeValue = sensor.MinRangeValue;
					model.MaxRangeValue = sensor.MaxRangeValue;
					model.ApiMaxRangeValue = sensor.IcbSensor.MaxRangeValue;
					model.ApiMinRangeValue = sensor.IcbSensor.MinRangeValue;
				}
				return View(model);
			}
			catch (EntityDoesntExistException e)
			{
				TempData["Error-Message"] = e.Message;
				return this.NotFound();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Update(CreateUpdateSensorViewModel model)
		{
			if (!this.ModelState.IsValid)
			{

				// TODO: Redirect to register index + temp data message
				return this.RedirectToAction("Details", "Sensor", new { sensorId = model.SensorId });
			}
			// TODO: Add validation for model, change user id to here, not from view
			// TODO: Tests

			var updatedSensorId = await this.sensorsService.Update(model.SensorId, model.UserId, model.IcbSensorId, model.Name, model.Description,
				model.PollingInterval, model.IsPublic, model.AlarmOn, model.MinRangeValue, model.MaxRangeValue,
				model.Longtitude, model.Latitude, model.SwitchOn);

			return this.RedirectToAction("Details", "Sensor", new { sensorId = updatedSensorId });
		}
	}
}
