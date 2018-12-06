using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartDormitory.App.Areas.Administration.Models.Sensor;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using System;
using System.Threading.Tasks;

namespace SmartDormitory.App.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = "Administrator")]
    public class SensorController : Controller
    {
        private const int PageSize = 2;
		// TODO : Details should be dynamic on the same details page
        private readonly ISensorsService sensorsService;
        private readonly IMeasureTypeService measureTypeService;

        public SensorController(ISensorsService sensorsService, IMeasureTypeService measureTypeService)
        {
            this.sensorsService = sensorsService;
            this.measureTypeService = measureTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var measureTypes = await this.measureTypeService
                                         .GetAll();
	
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
        public async Task<IActionResult> ToggleDelete(string sensorId)
        {
            try
            {
                await this.sensorsService.ToggleDeleteSensor(sensorId);
            }
            catch (EntityDoesntExistException e)
            {
                return this.NotFound(e.Message);
            }

            return this.Ok();
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
    }
}
