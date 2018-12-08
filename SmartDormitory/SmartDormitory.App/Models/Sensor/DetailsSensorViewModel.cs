using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.App.Models.Sensor
{
	public class DetailsSensorViewModel
	{
		public DetailsSensorViewModel()
		{

		}
		public string SensorId { get; set; }
		public string UserId { get; set; }
		[Required]
		[StringLength(30, ErrorMessage = "Sensor name should be between 3 and 30 symbols.", MinimumLength = 3)]
		public string Name { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "Sensor description should be between 3 and 30 symbols.", MinimumLength = 3)]
		public string Description { get; set; }

		[Required]
		public int PollingInterval { get; set; }
		public bool IsPublic { get; set; }
		public bool AlarmOn { get; set; }
		public float MinRangeValue { get; set; }
		public float MaxRangeValue { get; set; }
		public double Longtitude { get; set; }
		public double Latitude { get; set; }
		public float StartValue { get; set; }
		public string MeasureUnit { get; set; }
		public bool SwitchOn { get; set; }
	}
}
