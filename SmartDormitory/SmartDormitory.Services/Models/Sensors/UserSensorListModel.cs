using System;

namespace SmartDormitory.Services.Models.Sensors
{
    public class UserSensorListModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string SensorType { get; set; }

        public string MeasureUnit { get; set; }

        public int PollingInterval { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool AlarmOn { get; set; }

        public bool IsPublic { get; set; }

		public string Value { get; set; }

		public DateTime LastUpdateOn { get; set; }
	}
}
