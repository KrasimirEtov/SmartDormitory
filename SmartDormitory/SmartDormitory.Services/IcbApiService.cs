using Newtonsoft.Json;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.HttpClients;
using SmartDormitory.Services.Models.JsonDtoModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartDormitory.Services
{
    public class IcbApiService : IIcbApiService
    {
        private readonly IIcbHttpClient client;

        public IcbApiService(IIcbHttpClient client)
        {
            this.client = client;
        }

        public async Task<IReadOnlyList<ApiSensorDetailsDTO>> GetAllIcbSensors()
        {
            try
            {
                string jsonResult = await this.client.FetchAllSensors();

                return JsonConvert.DeserializeObject<IReadOnlyList<ApiSensorDetailsDTO>>(jsonResult);
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
        }

        public async Task<ApiSensorValueDTO> GetIcbSensorDataById(string id)
        {
            try
            {
                string jsonResult = await this.client.FetchSensorById(id);

                return JsonConvert.DeserializeObject<ApiSensorValueDTO>(jsonResult);
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
        }
    }
}
