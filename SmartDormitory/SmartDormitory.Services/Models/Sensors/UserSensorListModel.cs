using System;

namespace SmartDormitory.Services.Models.Sensors
{
    public class UserSensorListModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string SensorType { get; set; }

        public int PollingInterval { get; set; }

        public DateTime CreatedOn { get; set; }
        //todo add more
    }
}
