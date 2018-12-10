using SmartDormitory.Data.Models.Abstract;

namespace SmartDormitory.Data.Models
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public User Receiver { get; set; }

        public string ReceiverId { get; set; }

        public bool Seen { get; set; }

        public string SensorId { get; set; }

        public float AlarmValue { get; set; }
    }
}
