namespace SmartDormitory.Services.Models.Sensors
{
    public class AdminSensorListServiceModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public string OwnerUsername { get; set; }

        public string OwnerId { get; set; }

        public string SensorType { get; set; }
    }
}
