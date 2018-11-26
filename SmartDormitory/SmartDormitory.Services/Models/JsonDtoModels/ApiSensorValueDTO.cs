using Newtonsoft.Json;
using System;

namespace SmartDormitory.Services.Models.JsonDtoModels
{
    public class ApiSensorValueDTO
    {
        [JsonProperty("timeStamp", Required = Required.Always)]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("value", Required = Required.Always)]
        public string LastValue { get; set; }

        [JsonProperty("valueType", Required = Required.Always)]
        public string MeasurementUnit { get; set; }
    }
}
