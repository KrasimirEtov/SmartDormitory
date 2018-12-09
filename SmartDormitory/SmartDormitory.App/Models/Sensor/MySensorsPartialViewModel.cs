using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.App.Models.Sensor
{
	public class MySensorsPartialViewModel
	{
		public List<MySensorListViewModel> Sensors { get; set; }

		public int TotalPollingInterval { get; set; }
	}
}
