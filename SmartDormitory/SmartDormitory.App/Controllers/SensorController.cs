using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using SmartDormitory.App.Infrastructure.Extensions;
using SmartDormitory.App.Models.Sensor;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using SmartDormitory.Services.Models.MeasureTypes;
using System;
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
        private readonly IMemoryCache memoryCache;

        public SensorController(ISensorsService sensorsService, IIcbSensorsService icbSensorsService, IMeasureTypeService measureTypeService, IMemoryCache memoryCache)
        {
            this.sensorsService = sensorsService;
            this.icbSensorsService = icbSensorsService;
            this.measureTypeService = measureTypeService;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> MySensors()
        {
            var userId = this.User.GetId();
            var measureTypes = await this.CachedMeasureTypes();
            var sensors = await this.sensorsService.GetUserSensors(userId);

            var model = new MySensorsViewModel
            {
                MeasureTypes = new SelectList(measureTypes, "Id", "SuitableSensorType"),

                Sensors = sensors.Select(s => new MySensorListViewModel(s)).ToList()
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
            var measureTypes = await this.CachedMeasureTypes();
            var userId = this.User.GetId();

            var model = new IcbSensorTypesViewModel
            {
                MeasureTypes = new SelectList(measureTypes, "Id", "SuitableSensorType"),
                MeasureTypeId = string.Empty,
                IcbSensors = new List<IcbSensorsListViewModel>(),
                UserId = userId
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
                var userId = User.GetId();
                var model = sensors.Select(s => new IcbSensorsListViewModel(s, userId)).ToList();

                return PartialView("_IcbSensorsByTypeResult", model);
            }
            catch (EntityDoesntExistException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(string icbSensorId)
        {
            // TODO: Better implementation
            var icbSensor = await this.icbSensorsService.GetSensorById(icbSensorId);
            if (icbSensor == null)
            {
                // TODO: Error
                return this.NotFound();
            }

            var userId = User.GetId();

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
        public async Task<IActionResult> Details(string sensorId)
        {
            try
            {
                var sensor = await sensorsService.GetSensorById(sensorId);
                if (sensor.UserId != User.GetId() && !User.IsInRole("Administrator"))
                {
                    TempData["Error-Message"] = "Access denied!";
                    return RedirectToAction("Index", "Home");
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
                    StartValue = sensor.CurrentValue,
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
        public async Task<IActionResult> GetGaugeData(string sensorId)
        {
            try
            {
                var data = await sensorsService.GetGaugeData(sensorId);
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

                if (sensor.UserId != User.GetId())
                {
                    TempData["Error-Message"] = "Access denied!";
                    return RedirectToAction("Index", "Home");
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

        [NonAction]
        private async Task<IEnumerable<MeasureTypeServiceModel>> CachedMeasureTypes()
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            };

            if (!this.memoryCache.TryGetValue("MeasureTypes", out IEnumerable<MeasureTypeServiceModel> measureTypes))
            {
                measureTypes = await this.measureTypeService.GetAllNotDeleted();
                this.memoryCache.Set("MeasureTypes", measureTypes, cacheOptions);
            }

            return measureTypes;
        }

    }
}