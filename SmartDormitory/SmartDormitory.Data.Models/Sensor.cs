using SmartDormitory.Data.Models.Abstract;
using System;

namespace SmartDormitory.Data.Models
{
    public class Sensor : BaseEntity
    {
        public string OwnerId { get; set; }
        public User Owner { get; set; }

        public string ApiSensorId { get; set; }
        public ApiSensor ApiSensor { get; set; }

        public string Name { get; set; }

        public bool AlarmOn { get; set; } = false;

        public bool IsPublic { get; set; } = false;
    }
}
