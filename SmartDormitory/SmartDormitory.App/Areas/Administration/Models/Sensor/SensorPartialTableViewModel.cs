using SmartDormitory.Services.Models.Sensors;
using System.Collections.Generic;

namespace SmartDormitory.App.Areas.Administration.Models.Sensor
{
    public class SensorPartialTableViewModel
    {
        public IEnumerable<AdminListSensorModel> Sensors { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public int PreviousPage => this.CurrentPage == 1
            ? 1
            : this.CurrentPage - 1;

        public int NextPage => this.CurrentPage == this.TotalPages
            ? this.TotalPages
            : this.CurrentPage + 1;
    }
}
