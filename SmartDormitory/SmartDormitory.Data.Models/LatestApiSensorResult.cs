using SmartDormitory.Data.Models.Abstract;
using System;

namespace SmartDormitory.Data.Models
{
    public class LatestApiSensorResult : BaseEntity
    {
        public DateTime TimeStamp { get; set; }

        public double Value { get; set; }

        public string ApiSensorId { get; set; }

        public ApiSensor ApiSensor { get; set; }
    }
}
