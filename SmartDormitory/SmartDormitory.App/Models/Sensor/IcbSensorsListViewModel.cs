namespace SmartDormitory.App.Models.Sensor
{
    public class IcbSensorsListViewModel
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public int PollingInterval { get; set; }

        public string Tag { get; set; }
    }
}
