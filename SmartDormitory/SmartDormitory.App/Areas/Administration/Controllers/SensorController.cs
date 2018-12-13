using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartDormitory.App.Areas.Administration.Models.Sensor;
using SmartDormitory.App.Infrastructure.Common;
using SmartDormitory.App.Infrastructure.Extensions;
using SmartDormitory.App.Models.Sensor;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using SmartDormitory.Services.Models.IcbSensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.App.Areas.Administration.Controllers
{
    [Area(WebConstants.AdministrationArea)]
    [Authorize(Policy = WebConstants.AdminPolicy)]
    public class SensorController : Controller
    {
        private const int PageSize = 5;
        // TODO : Details should be dynamic on the same details page
        private readonly ISensorsService sensorsService;
        private readonly IIcbSensorsService icbSensorsService;
        private readonly IMeasureTypeService measureTypeService;

        public SensorController(ISensorsService sensorsService, IMeasureTypeService measureTypeService,
            IIcbSensorsService icbSensorsService)
        {
            this.sensorsService = sensorsService;
            this.measureTypeService = measureTypeService;
            this.icbSensorsService = icbSensorsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var measureTypes = await this.measureTypeService
                                         .GetAllNotDeleted();

            var model = new SensorsIndexViewModel
            {
                MeasureTypes = new SelectList(measureTypes, "Id", "SuitableSensorType"),
                PartialModel = await this.UpdateSensorsTableViewModel()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadSensorsTable(int page = 1, string measureTypeId = "all", int isPublic = -1, int alarmSet = -1)
        {
            var model = await this.UpdateSensorsTableViewModel(page, measureTypeId, isPublic, alarmSet);

            return PartialView("_SensorsTablePartialView", model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleDelete([FromForm]string sensorId)
        {
            try
            {
                await this.sensorsService.ToggleSoftDeleteSensor(sensorId);
            }
            catch (EntityDoesntExistException e)
            {
                return this.NotFound(e.Message);
            }

            return this.Ok();
        }

        [HttpGet]
        public async Task<IActionResult> RegisterIndex(string userId)
        {
            var sensorTypes = await this.measureTypeService.GetAllNotDeleted();
            var sensors = await this.icbSensorsService.GetSensorsByMeasureTypeId();

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
                return RedirectToAction("PageNotFound", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(string icbSensorId, string userId)
        {
            // TODO: Better implementation
            var icbSensor = await this.icbSensorsService.GetSensorById(icbSensorId);
            if (icbSensor == null)
            {
                // TODO: Error
                return this.NotFound();
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
                    userId = model.UserId,
                    area = "Administration"
                });
            }

            var createdSensorId = await this.sensorsService.RegisterNewSensor(model.UserId, model.IcbSensorId, model.Name,
                model.Description, model.PollingInterval, model.IsPublic,
                model.AlarmOn, model.MinRangeValue, model.MaxRangeValue,
                model.Longtitude, model.Latitude, model.SwitchOn);

            this.TempData["Success-Message"] = $"You successfully registered a new sensor!";
			return this.RedirectToAction("Create", "Sensor", new
			{
				icbSensorId = model.IcbSensorId,
				userId = model.UserId,
				area = "Administration"
			});
		}

        [HttpGet]
        public async Task<IActionResult> Update(string sensorId)
        {
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
				return this.RedirectToAction("Index", "Sensor", new { area = "Administration" });
			}
        }

        [HttpPost]
        public async Task<IActionResult> Update(CreateUpdateSensorViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                TempData["Error-Message"] = "Oops something went wrong! Try again..";
                return this.RedirectToAction("Update", "Sensor", new { sensorId = model.SensorId, area = "Administration" });
            }

            var updatedSensorId = await this.sensorsService.Update(model.SensorId, model.UserId, model.IcbSensorId, model.Name, model.Description,
                model.PollingInterval, model.IsPublic, model.AlarmOn, model.MinRangeValue, model.MaxRangeValue,
                model.Longtitude, model.Latitude, model.SwitchOn);
            this.TempData["Success-Message"] = $"You successfully updated the sensor!";

            return this.RedirectToAction("Index", "Sensor", new { area = "Administration" });
        }

        private async Task<SensorPartialTableViewModel> UpdateSensorsTableViewModel(int page = 1, string measureTypeId = "all", int isPublic = -1, int alarmSet = -1)
        {
            var sensors = await this.sensorsService.AllAdmin(measureTypeId, isPublic, alarmSet, page, PageSize);

            int totalSensors = await this.sensorsService.TotalSensorsByCriteria(measureTypeId, isPublic, alarmSet);

            var vm = new SensorPartialTableViewModel()
            {
                Sensors = sensors,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalSensors / (double)PageSize)
            };

            return vm;
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


    }
}
