﻿using Microsoft.EntityFrameworkCore;
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
                           OwnerId = s.OwnerId,
                           OwnerUsername = s.Owner.UserName,
                           IsPublic = s.IsPublic,
                           AlarmOn = s.AlarmOn
                       });
        }

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

        public async Task<string> RegisterNewSensor(string ownerId, string icbSensorId, string name, string description, int userPollingInterval, bool isPublic, bool alarmOn, float alarmMinRange, float alarmMaxRange, double longtitude, double latitude)
        {
            if (await this.Context.IcbSensors.AnyAsync(s => s.Id == icbSensorId))
            {
                var sensor = new Sensor()
                {
                    AlarmMaxRangeValue = alarmMaxRange,
                    AlarmMinRangeValue = alarmMinRange,
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
                    },
                    CreatedOn = DateTime.Now
                };
                await this.Context.Sensors.AddAsync(sensor);
                await this.Context.SaveChangesAsync();
                return sensor.Id;
            }

            return null;
        }

        public async Task<Sensor> GetSensorById(string sensorId)
        {
            return await this.Context.Sensors
                .Where(s => s.Id == sensorId)
                .FirstOrDefaultAsync();
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
    }
}
