using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Models.IcbSensors;
using SmartDormitory.Services.Models.JsonDtoModels;
using SmartDormitory.Services.Utils;
using SmartDormitory.Services.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Services
{
    public class IcbSensorsService : BaseService, IIcbSensorsService
    {
        private readonly IMeasureTypeService measureTypeService;

        public IcbSensorsService(SmartDormitoryContext context,
            IMeasureTypeService measureTypeService)
            : base(context)
        {
            this.measureTypeService = measureTypeService;
        }

        public async Task AddSensors(IReadOnlyList<ApiSensorDetailsDTO> lastApiSensors)
        {
            bool shouldCallSaveChanges = false;

            foreach (var newSensor in lastApiSensors)
            {
                var measureType = await this.Context
                                            .MeasureTypes
                                            .FirstOrDefaultAsync(mt => mt.MeasureUnit == newSensor.MeasureType && !mt.IsDeleted);

                if (measureType != null)
                {
                    var (MinRange, MaxRange) = ApiDataHelper
                                                    .GetMinAndMaxRange(newSensor.Description);

                    var icbSensorToAdd = new IcbSensor
                    {
                        Id = newSensor.ApiSensorId,
                        Description = newSensor.Description,
                        Tag = newSensor.Tag,
                        MeasureTypeId = measureType.Id,
                        PollingInterval = newSensor.MinPollingIntervalInSeconds,
                        MinRangeValue = MinRange,
                        MaxRangeValue = MaxRange,
                        CreatedOn = DateTime.Now
                    };

                    await this.Context.IcbSensors.AddAsync(icbSensorToAdd);

                    if (!shouldCallSaveChanges)
                        shouldCallSaveChanges = true;
                }
            }

            if (shouldCallSaveChanges)
                await this.Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<IcbSensorRegisterListServiceModel>> GetAllByMeasureTypeId(int page = 1, int pageSize = 10, string measureTypeId = "")
        {
            var sensors = this.Context.IcbSensors.Where(s => !s.IsDeleted);

            if (!string.IsNullOrWhiteSpace(measureTypeId))
            {
                if (await this.measureTypeService.Exists(measureTypeId))
                {
                    sensors = sensors.Where(s => s.MeasureTypeId == measureTypeId);
                }
            }
            else
            {
                return new List<IcbSensorRegisterListServiceModel>();
            }

            return await sensors
                              .OrderBy(s => s.MeasureType)
                              .ThenBy(s => s.Tag)
                              .ThenBy(s => s.PollingInterval)
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .Select(s => new IcbSensorRegisterListServiceModel
                              {
                                  Description = s.Description,
                                  Id = s.Id,
                                  PollingInterval = s.PollingInterval,
                                  Tag = s.Tag
                              })
                              .ToListAsync();
        }

        public async Task<IcbSensorCreateServiceModel> GetById(string sensorId)
        {
            Validator.ValidateNull(sensorId);
            Validator.ValidateGuid(sensorId);

            return await this.Context.IcbSensors
                .Where(s => !s.IsDeleted && s.Id == sensorId)
                .Select(s => new IcbSensorCreateServiceModel()
                {
                    Id = s.Id,
                    MinRangeValue = s.MinRangeValue,
                    MaxRangeValue = s.MaxRangeValue,
                    PollingInterval = s.PollingInterval,
                    MeasureType = s.MeasureType
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> TotalCount()
                => await this.Context
                             .IcbSensors
                             .Where(s => !s.IsDeleted)
                             .CountAsync();

        public async Task<bool> ExistsById(string sensorId)
        {
            Validator.ValidateNull(sensorId);
            Validator.ValidateGuid(sensorId);

            return await this.Context
                         .IcbSensors
                         .AnyAsync(s => !s.IsDeleted && s.Id == sensorId);
        }

        public async Task<IEnumerable<IcbSensor>> GetAll()
            => await this.Context
                        .IcbSensors
                        .Where(s => !s.IsDeleted)
                        .ToListAsync();
    }
}
