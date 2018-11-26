using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
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

        // return newly created icb sensors Guid Ids so we can add new RecurringJob for each of them
        public async Task<IReadOnlyList<string>> AddSensors()
        {
            var newSensorsIds = new List<string>();
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

                        this.Context.IcbSensors.Add(icbSensorToAdd);
                        newSensorsIds.Add(icbSensorToAdd.Id);
                    }
                }
            }

            await this.Context.SaveChangesAsync();

            return newSensorsIds;
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
