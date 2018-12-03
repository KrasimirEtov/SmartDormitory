using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using SmartDormitory.Services.Models.Sensors;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Services
{
    public class SensorsService : BaseService, ISensorsService
    {
        public SensorsService(SmartDormitoryContext context) : base(context)
        {
        }

        // for test purposes google map
        public void SeedSomeSensorsForMaps()
        {
            var sensor1 = new Sensor { Name = "Gerena sensors", Description = "suhata reka bla bla", Coordinates = new Coordinates { Latitude = 42.7034, Longitude = 23.36614 } };
            var sensor2 = new Sensor { Name = "Telerik academy", Description = "steven temp sensor", Coordinates = new Coordinates { Latitude = 42.6522, Longitude = 23.37397 } };
            var sensor3 = new Sensor { Name = "Letishte sofia", Description = "noise sensor terminal 1", Coordinates = new Coordinates { Latitude = 42.687333, Longitude = 23.405452 } };
            var sensor4 = new Sensor { Name = "Borisova gr", Description = "some sensor test 123", Coordinates = new Coordinates { Latitude = 42.679320, Longitude = 23.339460 } };
            this.Context.Sensors.Add(sensor1);
            this.Context.Sensors.Add(sensor2);
            this.Context.Sensors.Add(sensor3);
            this.Context.Sensors.Add(sensor4);
            this.Context.SaveChanges();
        }

        public async Task<IEnumerable<MapSensorServiceModel>> GetAllPublicSensorsCoordinates()
        {
            return await this.Context
                .Sensors
                .Select(s => new MapSensorServiceModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Coordinates = new Coordinates
                    {
                        Latitude = s.Coordinates.Latitude,
                        Longitude = s.Coordinates.Longitude
                    },
                })
                .ToListAsync();
        }

        public async Task RegisterNewSensor(string ownerId, string icbSensorId, string name, string description,
            int userPollingInterval, bool isPublic, bool alarmOn, float AlarmMinRange, float AlarmMaxRange,
            double longtitude, double latitude)
        {
            // TODO: Create a model for this long parameters list
            var sensor = new Sensor()
            {
                AlarmMaxRangeValue = AlarmMaxRange,
                AlarmMinRangeValue = AlarmMinRange,
                AlarmOn = alarmOn,
                Description = description,
                Name = name,
                OwnerId = ownerId,
                IsPublic = isPublic,
                UserPollingInterval = userPollingInterval,
                IcbSensorId = icbSensorId,
                Coordinates = new Coordinates
                {
                    Latitude = latitude,
                    Longitude = longtitude
                }
            };
            await this.Context.Sensors.AddAsync(sensor);
            await this.Context.SaveChangesAsync();
        }

        public IEnumerable<AdminSensorListServiceModel> AllAdmin()
            => this.Context
                    .Sensors
                    //.Include(s => s.Owner)
                    .Select(s => new AdminSensorListServiceModel
                    {
                        Id = s.Id,
                        Name = s.Name,
                        IsDeleted = s.IsDeleted,
                        SensorType = s.IcbSensor.MeasureType.SuitableSensorType,
                        OwnerId = s.OwnerId,
                        OwnerUsername = s.Owner.UserName
                    });

        public async Task ToggleDeleteSensor(string sensorId)
        {
            var sensor = await this.Context.Sensors.FirstOrDefaultAsync(s => s.Id == sensorId);

            if (sensor is null)
                throw new EntityDoesntExistException(nameof(Sensor).ToLower(), sensorId);

            if (sensor.IsDeleted == false)
            {
                sensor.IsDeleted = true;
            }
            else
            {
                sensor.IsDeleted = false;
            }

            await this.Context.SaveChangesAsync();
        }

        public async Task RestoreSensor(string sensorId)
        {
            var sensor = await this.Context.Sensors.FirstOrDefaultAsync(s => s.Id == sensorId);

            if (sensor is null)
                throw new EntityDoesntExistException(nameof(Sensor).ToLower(), sensorId);

            if (sensor.IsDeleted == true)
            {
                sensor.IsDeleted = false;
                await this.Context.SaveChangesAsync();
            }
        }
    }
}
