using Newtonsoft.Json;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.HttpClients;
using SmartDormitory.Services.Models.JsonDtoModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services
{
    public class IcbApiService : IIcbApiService
    {
        private readonly IcbHttpClient client;

        public IcbApiService(IcbHttpClient client)
        {
            this.client = client;
        }

        public async Task<IReadOnlyList<ApiSensorDetailsDTO>> GetAllIcbSensors()
        {
            string jsonResult = await this.client.FetchAllSensors();

            return JsonConvert.DeserializeObject<IReadOnlyList<ApiSensorDetailsDTO>>(jsonResult);
        }

        public async Task<ApiSensorValueDTO> GetIcbSensorValueById(string id)
        {
            string jsonResult = await this.client.FetchSensorById(id);

            return JsonConvert.DeserializeObject<ApiSensorValueDTO>(jsonResult);
        }
    }
}
