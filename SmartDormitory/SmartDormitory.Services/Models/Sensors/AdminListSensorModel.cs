namespace SmartDormitory.Services.Models.Sensors
{
    public class AdminListSensorModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public string OwnerUsername { get; set; }

        public string OwnerId { get; set; }

        public string SensorType { get; set; }

        public bool IsPublic { get; set; }

        public bool AlarmOn { get; set; }
    }
}
