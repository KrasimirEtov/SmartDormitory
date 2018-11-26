using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartDormitory.Services
{
    public class IcbSensorsService : BaseService, IIcbSensorsService
    {
        private readonly IIcbApiService icbApiService;

        public IcbSensorsService(SmartDormitoryContext context, IIcbApiService icbApiService) : base(context)
        {
            this.icbApiService = icbApiService;
        }

        // return newly created icb sensors Guid Ids and PollingInterval so we can add new RecurringJob for each of them
        public async Task<IEnumerable<(string Id, int PollingInterval)>> AddSensorsAsync()
        {
            var createdSensorsJobData = new List<(string Id, int PollingInterval)>();
            var icbSensors = await this.icbApiService.GetAllIcbSensors();

            foreach (var icbSensor in icbSensors)
            {
                bool sensorExists = await this.Context
                                              .IcbSensors
                                              .AnyAsync(s => s.Id == icbSensor.ApiSensorId && !s.IsDeleted);

                if (!sensorExists)
                {
                    var measureType = await this.Context
                                                .MeasureTypes
                                                .FirstOrDefaultAsync(mt =>
                                                                         mt.MeasureUnit == icbSensor.MeasureType
                                                                            && !mt.IsDeleted);

                    if (measureType != null)
                    {
                        var (MinRange, MaxRange) = ExtractMinAndMaxRange(icbSensor.Description);

                        var icbSensorToAdd = new IcbSensor
                        {
                            Id = icbSensor.ApiSensorId,
                            Description = icbSensor.Description,
                            Tag = icbSensor.Tag,
                            MeasureTypeId = measureType.Id,
                            PollingInterval = icbSensor.MinPollingIntervalInSeconds,
                            MinRangeValue = MinRange,
                            MaxRangeValue = MaxRange
                        };

                        await this.Context.IcbSensors.AddAsync(icbSensorToAdd);
                        createdSensorsJobData.Add((icbSensorToAdd.Id, icbSensorToAdd.PollingInterval));
                    }
                }
            }

            await this.Context.SaveChangesAsync();

            return createdSensorsJobData;
        }

        public async Task UpdateSensorValueAsync(string id, DateTime timeStamp, string lastValue, string measureUnit)
        {
            var icbSensor = await this.Context
                                      .IcbSensors
                                      .Include(s => s.MeasureType)
                                      .FirstOrDefaultAsync(s => s.Id == id);

            if (icbSensor != null)
            {
                if (icbSensor.MeasureType.MeasureUnit == measureUnit)
                {
                    icbSensor.LastUpdateOn = timeStamp;
                    icbSensor.CurrentValue = this.ExtractLastValue(lastValue);

                    await this.Context.SaveChangesAsync();
                }
            }
            // return sensor?
        }

        //public IEnumerable<string> HangfireGetAll()
        //    => this.Context.IcbSensors.Select(s => s.Id);

        private float ExtractLastValue(string lastValue)
        {
            bool isParsable = float.TryParse(lastValue, out float value);

            if (!isParsable)
            {
                return lastValue.Equals("true") ? 1 : lastValue.Equals("false") ? 0 : throw new InvalidOperationException("Invalid last value response");
            }

            return value;
        }

        private (int MinRange, int MaxRange) ExtractMinAndMaxRange(string description)
        {
            if (description.Any(char.IsDigit))
            {
                var numbers = Regex.Matches(description, @"\d+");

                int minRange = int.Parse(numbers[0].Value);
                int maxRange = int.Parse(numbers[1].Value);

                return (MinRange: minRange, MaxRange: maxRange);
            }
            //              false    true
            return (MinRange: 0, MaxRange: 1);
        }
    }
}
