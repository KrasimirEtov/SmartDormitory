using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface IIcbSensorsService
    {
        Task<IEnumerable<(string Id, int PollingInterval)>> AddSensorsAsync();
        Task UpdateSensorValueAsync(string id, DateTime timeStamp, string lastValue, string measUnit);
       
    }
}