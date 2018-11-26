using Newtonsoft.Json;

namespace SmartDormitory.Services.Models.JsonDtoModels
{
    public class ApiSensorDetailsDTO
    {
        [JsonProperty("sensorId")]
        public string ApiSensorId { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("minPollingIntervalInSeconds")]
        public int MinPollingIntervalInSeconds { get; set; }

        [JsonProperty("measureType")]
        public string MeasureType { get; set; }
    }
}
