using SmartDormitory.Services.Models.IcbSensors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface IIcbSensorsService
    {
        Task<IEnumerable<(string Id, int PollingInterval)>> AddSensorsAsync();

        Task UpdateSensorValueAsync(string id, DateTime timeStamp, string lastValue, string measUnit);

        Task<IEnumerable<IcbSensorRegisterListServiceModel>> GetSensorsByMeasureTypeId(string measureTypeId = "all");

        Task<IcbSensorCreateServiceModel> GetSensorById(string sensorId);
    }
}