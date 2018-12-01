using SmartDormitory.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDormitory.Services.Models.IcbSensors
{
	public class IcbSensorCreateServiceModel
	{
		public string Id { get; set; }
		public float MinRangeValue { get; set; }
		public float MaxRangeValue { get; set; }
		public int PollingInterval { get; set; }
		public MeasureType MeasureType { get; set; }
	}
}
