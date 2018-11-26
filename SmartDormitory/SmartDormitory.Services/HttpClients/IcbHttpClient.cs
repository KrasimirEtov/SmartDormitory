using System;
using System.Net.Http;
using System.Threading.Tasks;
using static SmartDormitory.Services.Utils.Constants.IcbApi;

namespace SmartDormitory.Services.HttpClients
{
    public class IcbHttpClient : HttpClient
    {
        private readonly HttpClient client;

        public IcbHttpClient(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.client.BaseAddress = new Uri(BaseUrl);
            this.client.DefaultRequestHeaders.Accept.Clear();
            this.client.DefaultRequestHeaders.Add(AcceptHeaderKey, AcceptHeaderValue);
            this.client.DefaultRequestHeaders.Add(AuthorizationHeaderKey, AuthorizationHeaderValue);
        }

        public async Task<string> FetchAllSensors()
                => await this.FetchData(IcbSensorPostfix + AllSensorsPostfix);

        public async Task<string> FetchSensorById(string id)
                => await this.FetchData(IcbSensorPostfix + id);

        private async Task<string> FetchData(string requestUri)
        {
            try
            {
                var response = await this.client.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsStringAsync().Result;
            }
            catch (HttpRequestException)
            {
                //this.logger.LogError($"An error occured connecting to values API {ex.ToString()}");

                // throw some and handle to show UI api is down
                return "ERROR";
            }
        }
    }
}