using System;
using System.Net.Http;
using System.Threading.Tasks;
using static SmartDormitory.Services.Utils.Constants;

namespace SmartDormitory.Services.HttpClients
{
    public class IcbHttpClient : HttpClient
    {
        private readonly HttpClient client;

        public IcbHttpClient(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.client.BaseAddress = new Uri(IcbApi.BaseUrl);
            this.client.DefaultRequestHeaders.Accept.Clear();
            this.client.DefaultRequestHeaders.Add(IcbApi.AcceptHeaderKey, IcbApi.AcceptHeaderValue);
            this.client.DefaultRequestHeaders.Add(IcbApi.AuthorizationHeaderKey, IcbApi.AuthorizationHeaderValue);
        }

        public async Task<string> FetchAllSensors()
                => await this.FetchData(IcbApi.IcbSensorPostfix + IcbApi.AllSensorsPostfix);

        public async Task<string> FetchSensorById(string id)
                => await this.FetchData(IcbApi.IcbSensorPostfix + id);

        private async Task<string> FetchData(string requestUri)
        {
            var response = await this.client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}