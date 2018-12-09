using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SmartDormitory.App.Models.Sensor
{
    public class IcbSensorTypesViewModel
    {
        public string MeasureTypeId { get; set; } = string.Empty;

        public SelectList MeasureTypes { get; set; }

        public IEnumerable<IcbSensorsListViewModel> IcbSensors { get; set; }
		public string UserId { get; set; }
	}
}
