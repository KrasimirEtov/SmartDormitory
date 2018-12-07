using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartDormitory.Services.Models.Sensors
{
	public class GaugeDataServiceModel
	{
		public int UserPollingInterval { get; set; }
		public int ApiPollingInterval { get; set; }

		public float UserMinRangeValue { get; set; }
		public float UserMaxRangeValue { get; set; }
		public float ApiMinRangeValue { get; set; }
		public float ApiMaxRangeValue { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime ApiLastUpdateOn { get; set; }
		public DateTime UserLastUpdateOn { get; set; }

		public float ApiCurrentValue { get; set; }
		public float UserCurrentValue { get; set; }
		public string MeasureUnit { get; set; }

	}
}
