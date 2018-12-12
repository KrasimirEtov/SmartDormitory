using SmartDormitory.Services.Models.IcbSensors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface IIcbSensorsService
    {
        Task<IEnumerable<(string Id, int PollingInterval)>> AddSensorsAsync();

        Task<IEnumerable<IcbSensorRegisterListServiceModel>> GetSensorsByMeasureTypeId(int page = 1, int pageSize = 10, string measureTypeId = "");

        Task<IcbSensorCreateServiceModel> GetSensorById(string sensorId);

        Task<int> TotalCount();
    }
}