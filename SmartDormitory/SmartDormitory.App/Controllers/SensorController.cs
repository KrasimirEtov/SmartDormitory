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

		[HttpGet]
		public async Task<IActionResult> MySensors()
		{
			var userId = this.User.GetId();
			var measureTypes = await this.measureTypeService.GetAll();

			var sensors = await this.sensorsService.GetUserSensors(userId);

			var model = new MySensorsViewModel
			{
				MeasureTypes = new SelectList(measureTypes, "Id", "SuitableSensorType"),

				MySensorsPartialViewModel = new MySensorsPartialViewModel
				{
					Sensors = sensors.Select(s => new MySensorListViewModel(s)).ToList()
				}
			};
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> ReloadMySensorsTable(string measureTypeId = "all", string searchTerm = "",
			int alarmOn = -1, int privacy = -1)
		{
			try
			{
				var userId = this.User.GetId();

				var model = new MySensorsPartialViewModel
				{
					Sensors = (await this.sensorsService
										 .GetUserSensors(userId, searchTerm, measureTypeId,
																alarmOn, privacy))
										.Select(s => new MySensorListViewModel(s))
										.ToList()
				};

				return PartialView("_MySensorsTable", model);
			}
			catch (EntityDoesntExistException e)
			{
				return NotFound(e.Message);
			}
		}

		[HttpGet]
		public async Task<IActionResult> RegisterIndex(string userId)
		{
			var sensorTypes = await this.measureTypeService.GetAll();
			var sensors = await this.icbSensorsService.GetSensorsByMeasureTypeId();

			if (!UserHasAccess(userId))
			{
				TempData["Error-Message"] = "Access denied!";
				return this.RedirectToAction("Index", "Home");
			}
			var model = new IcbSensorTypesViewModel
			{
				MeasureTypes = new SelectList(sensorTypes, "Id", "SuitableSensorType"),
				MeasureTypeId = string.Empty,
				IcbSensors = this.MapSensorServiceModelToViewModel(sensors, userId),
				UserId = userId
			};

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> LoadSensorsByType(string measureTypeId, string userId, int page = 1)
		{
			try
			{
				var sensors = await this.icbSensorsService
					.GetSensorsByMeasureTypeId(page, PageSize, measureTypeId);
				var model = this.MapSensorServiceModelToViewModel(sensors, userId);

				return PartialView("_IcbSensorsByTypeResult", model);
			}
			catch (EntityDoesntExistException e)
			{
				return NotFound(e.Message);
			}
		}

		//[HttpGet]
		//public async Task<IActionResult> GetSensorsValue(string measureTypeId = "all", string searchTerm = "",
		//	int alarmOn = -1, int privacy = -1)
		//{
		//	try
		//	{
		//		//var userId = this.User.GetId();
		//		//var sensors = (await this.sensorsService
		//		//						 .GetUserSensors(userId, searchTerm, measureTypeId,
		//		//												alarmOn, privacy))
		//		//						.Select(s => new MySensorListViewModel(s))
		//		//						.ToList();
		//		var userId = User.GetId();
		//		var sensors = (await sensorsService
		//			.GetUserSensors(userId, searchTerm, measureTypeId, alarmOn, privacy))
		//			.Select(s => new MySensorListViewModel(s))
		//			.ToList();

		//		//return Json(data);
		//	}
		//	catch (EntityDoesntExistException e)
		//	{
		//		TempData["Error-Message"] = e.Message;
		//		return this.RedirectToAction("Index", "Home");
		//	}
		//}

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
				if (!UserHasAccess(userId))
				{
					TempData["Error-Message"] = "Access denied!";
					return this.RedirectToAction("Index", "Home");
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
				TempData["Error-Message"] = "Error while trying to create a new sensor";
				return this.RedirectToAction("Create", "Sensor", new
				{
					icbSensorId = model.IcbSensorId,
					userId = model.UserId
				});
			}

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
			IEnumerable<IcbSensorRegisterListServiceModel> sensors, string userId)
		{
			return sensors.Select(s => new IcbSensorsListViewModel
			{
				Id = s.Id,
				Description = s.Description,
				PollingInterval = s.PollingInterval,
				Tag = s.Tag.SplitTag(),
				UserId = userId
				//set image url depends on tag
			}).ToList();
		}

		[HttpGet]
		public async Task<IActionResult> Details(string sensorId)
		{
			try
			{
				var sensor = await sensorsService.GetSensorById(sensorId);
				if (!UserHasAccess(sensor.UserId))
				{
					TempData["Error-Message"] = "Access denied!";
					return this.RedirectToAction("Index", "Home");
				}
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
					StartValue = sensor.IcbSensor.CurrentValue,
					MeasureUnit = sensor.IcbSensor.MeasureType.MeasureUnit,
					SwitchOn = sensor.SwitchOn,
					IsSwitch = sensor.IcbSensor.MeasureType.MeasureUnit == "(true/false)" ? true : false

				};
				return View(model);
			}
			catch (EntityDoesntExistException e)
			{
				TempData["Error-Message"] = e.Message;
				return this.RedirectToAction("Index", "Home");
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
				return this.RedirectToAction("Index", "Home");
			}
		}

		[HttpGet]
		public async Task<IActionResult> Update(string sensorId)
		{
			try
			{
				var sensor = await this.sensorsService.GetSensorById(sensorId);

				if (!UserHasAccess(sensor.UserId))
				{
					TempData["Error-Message"] = "Access denied!";
					return this.RedirectToAction("Index", "Home");
				}
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
				return this.RedirectToAction("Index", "Home");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Update(CreateUpdateSensorViewModel model)
		{
			if (!this.ModelState.IsValid)
			{

				// TODO: Redirect to register index + temp data message
				TempData["Error-Message"] = "Oops something went wrong! Try again..";
				return this.RedirectToAction("Update", "Sensor", new { sensorId = model.SensorId });
			}

			var updatedSensorId = await this.sensorsService.Update(model.SensorId, model.UserId, model.IcbSensorId, model.Name, model.Description,
				model.PollingInterval, model.IsPublic, model.AlarmOn, model.MinRangeValue, model.MaxRangeValue,
				model.Longtitude, model.Latitude, model.SwitchOn);
			this.TempData["Success-Message"] = $"You successfully updated your sensor!";

			return this.RedirectToAction("Details", "Sensor", new { sensorId = updatedSensorId });
		}

		private bool UserHasAccess(string userId)
		{
			string roleName = "Administrator";
			if (User.GetId() == userId || User.IsInRole(roleName))
			{
				return true;
			}
			return false;

		}
	}
}
