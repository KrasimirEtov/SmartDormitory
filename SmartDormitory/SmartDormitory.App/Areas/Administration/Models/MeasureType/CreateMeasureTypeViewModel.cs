using SmartDormitory.Services.Models.MeasureTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartDormitory.App.Areas.Administration.Models.MeasureType
{
	public class CreateMeasureTypeViewModel
	{
		[Required]
		public string MeasureUnit { get; set; }

		[Required]
		public string SuitableSensorType { get; set; }

		public IEnumerable<MeasureTypeServiceModel> MeasureTypes { get; set; }
		
	}
}
