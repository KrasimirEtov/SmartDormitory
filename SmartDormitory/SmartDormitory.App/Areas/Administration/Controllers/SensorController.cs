using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Areas.Administration.Models.Sensor;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.App.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = "Administrator")]
    public class SensorController : Controller
    {
        private readonly ISensorsService sensorsService;

        public SensorController(ISensorsService sensorsService)
        {
            this.sensorsService = sensorsService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var sensors = this.sensorsService
                .AllAdmin()
                .Select(s => new SensorListViewModel
                {
                    Id = s.Id,
                    SensorType = s.SensorType,
                    IsDeleted = s.IsDeleted,
                    Name = s.Name,
                    OwnerId = s.OwnerId,
                    OwnerUsername = s.OwnerUsername
                });

            return View(sensors);
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

        [HttpPost]
        public async Task<IActionResult> Restore(string sensorId)
        {
            try
            {
                await this.sensorsService.RestoreSensor(sensorId);
            }
            catch (EntityDoesntExistException e)
            {
                return this.NotFound(e.Message);
            }

            return this.Ok();
        }
    }
}
