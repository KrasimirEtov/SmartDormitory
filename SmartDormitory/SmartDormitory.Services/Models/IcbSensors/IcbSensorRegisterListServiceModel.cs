namespace SmartDormitory.Services.Models.IcbSensors
{
    public class IcbSensorRegisterListServiceModel
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public int PollingInterval { get; set; }

        public string Tag { get; set; }
    }
}
