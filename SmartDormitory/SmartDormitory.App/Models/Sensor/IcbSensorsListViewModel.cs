using SmartDormitory.App.Infrastructure.Extensions;
using SmartDormitory.Services.Models.IcbSensors;

namespace SmartDormitory.App.Models.Sensor
{
    public class IcbSensorsListViewModel
    {
        public IcbSensorsListViewModel()
        {

        }

        public IcbSensorsListViewModel(IcbSensorRegisterListServiceModel sensor, string userId)
        {
            this.Id = sensor.Id;
            this.Description = sensor.Description;
            this.PollingInterval = sensor.PollingInterval;
            this.Tag = sensor.Tag.SplitTag();
            this.UserId = userId;
        }

        public string Id { get; set; }

        public string Description { get; set; }

        public int PollingInterval { get; set; }

        public string Tag { get; set; }

        public string UserId { get; set; }
    }
}
