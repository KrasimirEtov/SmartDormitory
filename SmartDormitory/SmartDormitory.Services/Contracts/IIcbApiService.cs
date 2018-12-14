using SmartDormitory.Services.Models.JsonDtoModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface IIcbApiService
    {
        Task<IReadOnlyList<ApiSensorDetailsDTO>> GetAllIcbSensors();
        Task<ApiSensorValueDTO> GetIcbSensorDataById(string id);
    }
}