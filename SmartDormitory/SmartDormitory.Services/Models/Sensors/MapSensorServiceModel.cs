using SmartDormitory.Data.Models;

namespace SmartDormitory.Services.Models.Sensors
{
    public class MapSensorServiceModel
    {
        public Coordinates Coordinates { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Id { get; set; }
    }
}
