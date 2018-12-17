using System;

namespace SmartDormitory.Services.Models.Notifications
{
    public class InboxServiceModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public float AlarmValue { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool Seen { get; set; }

        public string MeasureUnit { get; set; }

        public string SensorId { get; set; }

        public string SensorName { get; set; }
    }
}
