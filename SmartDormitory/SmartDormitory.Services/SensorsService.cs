using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using SmartDormitory.Services.Models.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Services
{
    public class SensorsService : BaseService, ISensorsService
    {
        private readonly IMeasureTypeService measureTypeService;

        public SensorsService(
            SmartDormitoryContext context,
            IMeasureTypeService measureTypeService)
            : base(context)
        {
            this.measureTypeService = measureTypeService;
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

        public async Task<IEnumerable<AdminListSensorModel>> AllAdmin(string measureTypeId = "all", int isPublic = -1, int alarmSet = -1, int page = 1, int pageSize = 10)
        {
            var sensors = this.Context.Sensors.AsQueryable();

            if (measureTypeId != "all")
            {
                if (await this.measureTypeService.Exists(measureTypeId))
                {
                    sensors = sensors.Where(s => s.IcbSensor.MeasureTypeId == measureTypeId);
                }
            }
            if (isPublic != -1 && (isPublic == 0 || isPublic == 1))
            {
                bool isPublicValue = isPublic != 0;
                sensors = sensors.Where(s => s.IsPublic == isPublicValue);
            }
            if (alarmSet != -1 && (alarmSet == 0 || alarmSet == 1))
            {
                bool alarmSetValue = alarmSet != 0;
                sensors = sensors.Where(s => s.AlarmOn == alarmSetValue);
            }

            return sensors
                       .OrderByDescending(s => s.CreatedOn)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .Select(s => new AdminListSensorModel
                       {
                           Id = s.Id,
                           Name = s.Name,
                           IsDeleted = s.IsDeleted,
                           SensorType = s.IcbSensor.MeasureType.SuitableSensorType,
                           OwnerId = s.UserId,
                           OwnerUsername = s.User.UserName,
                           IsPublic = s.IsPublic,
                           AlarmOn = s.AlarmOn
                       });
        }

        public async Task ToggleSoftDeleteSensor(string sensorId)
        {
            var sensor = await this.Context.Sensors.FirstOrDefaultAsync(s => s.Id == sensorId);

            if (sensor is null)
            {
                throw new EntityDoesntExistException(nameof(Sensor).ToLower(), sensorId);
            }

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

        public async Task<string> RegisterNewSensor(string userId, string icbSensorId, string name, string description,
            int pollingInterval, bool isPublic, bool alarmOn, float minRange,
            float maxRange, double longtitude, double latitude, bool switchOn)
        {
            string returnSensorId = "";
            if (await this.Context.IcbSensors.AnyAsync(s => s.Id == icbSensorId))
            {
                var sensor = new Sensor()
                {
                    MaxRangeValue = maxRange,
                    MinRangeValue = minRange,
                    AlarmOn = alarmOn,
                    Description = description,
                    Name = name,
                    UserId = userId,
                    IsPublic = isPublic,
                    PollingInterval = pollingInterval,
                    IcbSensorId = icbSensorId,
                    Coordinates = new Coordinates
                    {
                        Latitude = latitude,
                        Longitude = longtitude
                    },
                    CreatedOn = DateTime.Now,
                    SwitchOn = switchOn
                };
                await this.Context.Sensors.AddAsync(sensor);
                await this.Context.SaveChangesAsync();
                returnSensorId = sensor.Id;
            }
            return returnSensorId;
        }

        public async Task<Sensor> GetSensorById(string sensorId)
        {
            var sensor = await this.Context.Sensors
                .Where(s => s.Id == sensorId)
                .Include(icb => icb.IcbSensor)
                .ThenInclude(mt => mt.MeasureType)
                .FirstOrDefaultAsync();
            if (sensor == null || sensor.IsDeleted)
            {
                throw new EntityDoesntExistException("Sensor is no present in database!");
            }
            return sensor;
        }

        public async Task<int> TotalSensorsByCriteria(string measureTypeId = "all", int isPublic = -1, int alarmSet = -1)
        {
            var sensors = this.Context.Sensors.AsQueryable();

            if (measureTypeId != "all")
            {
                if (await this.measureTypeService.Exists(measureTypeId))
                {
                    sensors = sensors.Where(s => s.IcbSensor.MeasureTypeId == measureTypeId);
                }
            }
            if (isPublic != -1 && (isPublic == 0 || isPublic == 1))
            {
                bool isPublicValue = isPublic != 0;
                sensors = sensors.Where(s => s.IsPublic == isPublicValue);
            }
            if (alarmSet != -1 && (alarmSet == 0 || alarmSet == 1))
            {
                bool alarmSetValue = alarmSet != 0;
                sensors = sensors.Where(s => s.AlarmOn == alarmSetValue);
            }

            return await sensors.CountAsync();
        }

        public async Task<int> TotalSensors()
            => await this.Context.Sensors.CountAsync(s => s.IsDeleted == false);

        public async Task<IEnumerable<UserSensorListModel>> GetUserSensors(string userId, string searchTerm = "", string measureTypeId = "all", int alarmOn = -1, int isPublic = -1)
        {
            var user = this.Context.Users.FirstOrDefault(u => u.Id == userId);
            if (user is null)
            {
                throw new EntityDoesntExistException("User does not exist");
            }

            var sensors = this.Context.Sensors.Where(s => s.UserId == userId && !s.IsDeleted);

            if (measureTypeId != "all")
            {
                if (await this.measureTypeService.Exists(measureTypeId))
                {
                    sensors = sensors.Where(s => s.IcbSensor.MeasureTypeId == measureTypeId);
                }
            }
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                sensors = sensors.Where(s => s.Name.Contains(searchTerm) ||
                                           s.Description.Contains(searchTerm));
            }
            if (isPublic != -1 && (isPublic == 0 || isPublic == 1))
            {
                bool isPublicValue = isPublic != 0;
                sensors = sensors.Where(s => s.IsPublic == isPublicValue);
            }
            if (alarmOn != -1 && (alarmOn == 0 || alarmOn == 1))
            {
                bool alarmSetValue = alarmOn != 0;
                sensors = sensors.Where(s => s.AlarmOn == alarmSetValue);
            }

            return await sensors
                             .OrderByDescending(s => s.CreatedOn)
                             .Select(s => new UserSensorListModel
                             {
                                 Id = s.Id,
                                 Name = s.Name,
                                 SensorType = s.IcbSensor.MeasureType.SuitableSensorType,
                                 MeasureUnit = s.IcbSensor.MeasureType.MeasureUnit,
                                 PollingInterval = s.PollingInterval,
                                 CreatedOn = (DateTime)s.CreatedOn,
                                 IsPublic = s.IsPublic,
                                 AlarmOn = s.AlarmOn
                             })
                             .ToListAsync();
        }

        public async Task<GaugeDataServiceModel> GetGaudeData(string sensorId)
        {
            var sensor = await GetSensorById(sensorId);
            if (sensor == null || sensor.IsDeleted)
            {
                throw new EntityDoesntExistException("Sensor does not exist");
            }

            var model = new GaugeDataServiceModel()
            {
                ApiCurrentValue = sensor.IcbSensor.CurrentValue,
                ApiLastUpdateOn = sensor.IcbSensor.LastUpdateOn,
                ApiMaxRangeValue = sensor.IcbSensor.MaxRangeValue,
                ApiMinRangeValue = sensor.IcbSensor.MinRangeValue,
                ApiPollingInterval = sensor.IcbSensor.PollingInterval,
                UserCurrentValue = sensor.IcbSensor.CurrentValue,
                UserLastUpdateOn = DateTime.Now,
                UserPollingInterval = sensor.PollingInterval,
                UserMaxRangeValue = sensor.MaxRangeValue,
                UserMinRangeValue = sensor.MinRangeValue,
                MeasureUnit = sensor.IcbSensor.MeasureType.MeasureUnit
            };
            return model;
        }

        public async Task<string> Update(string sensorId, string userId, string icbSensorId, string name, string description,
            int pollingInterval, bool isPublic, bool alarmOn, float minRange,
            float maxRange, double longtitude, double latitude, bool switchOn)
        {
            var sensor = await this.GetSensorById(sensorId);

            if (sensor == null || sensor.IsDeleted)
            {
                throw new EntityDoesntExistException($"\nSensor is not present in the database.");
            }

            sensor.UserId = userId;
            sensor.IcbSensorId = icbSensorId;
            sensor.Name = name;
            sensor.Description = description;
            sensor.PollingInterval = pollingInterval;
            sensor.IsPublic = isPublic;
            sensor.AlarmOn = alarmOn;
            sensor.MinRangeValue = minRange;
            sensor.MaxRangeValue = maxRange;
            sensor.Coordinates.Longitude = longtitude;
            sensor.Coordinates.Latitude = latitude;
            sensor.ModifiedOn = DateTime.Now;
            sensor.SwitchOn = switchOn;

            this.Context.Sensors.Update(sensor);
            await this.Context.SaveChangesAsync();
            return sensor.Id;
        }

        public async Task<IEnumerable<Sensor>> GetAll()
            => await this.Context
                         .Sensors
                         .Where(s => !s.IsDeleted)
                         .OrderBy(s => s.IcbSensorId)
                         .ToListAsync();

        public async Task UpdateRange(IEnumerable<Sensor> sensorsToUpdate)
        {
            this.Context.UpdateRange(sensorsToUpdate);
            await this.Context.SaveChangesAsync();
        }
    }
}
