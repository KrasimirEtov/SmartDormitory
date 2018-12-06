using Microsoft.AspNetCore.Mvc.Rendering;
using SmartDormitory.Services.Models.Sensors;
using System.Collections.Generic;

namespace SmartDormitory.App.Models.Sensor
{
    public class MySensorsViewModel
    {
        public string MeasureTypeId { get; set; } = string.Empty;

        public SelectList MeasureTypes { get; set; }

        public int Privacy { get; set; } = -1;

        public int AlarmOn { get; set; } = -1;

        public IEnumerable<UserSensorListModel> Sensors { get; set; }
    }
}
