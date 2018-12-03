namespace SmartDormitory.App.Areas.Administration.Models.Sensor
{
    public class SensorListViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public string OwnerUsername { get; set; }

        public string OwnerId { get; set; }

        public string SensorType { get; set; }
    }
}
