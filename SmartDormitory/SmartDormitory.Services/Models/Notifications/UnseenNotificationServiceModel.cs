using System;

namespace SmartDormitory.Services.Models.Notifications
{
    public class UnseenNotificationServiceModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string ReceiverId { get; set; }

        public string SensorId { get; set; }

        public bool Seen { get; set; }

        public float AlarmValue { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
