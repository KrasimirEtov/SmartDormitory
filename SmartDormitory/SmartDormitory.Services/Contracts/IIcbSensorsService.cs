using SmartDormitory.Services.Models.IcbSensors;
using SmartDormitory.Services.Models.JsonDtoModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface IIcbSensorsService
    {
        Task AddSensorsAsync(IReadOnlyList<ApiSensorDetailsDTO> lastApiSensors);

        Task<IEnumerable<IcbSensorRegisterListServiceModel>> GetAllByMeasureTypeId(int page = 1, int pageSize = 10, string measureTypeId = "");

        Task<IcbSensorCreateServiceModel> GetById(string sensorId);

        Task<int> TotalCount();

        Task<bool> ExistsById(string sensorId);
    }
}