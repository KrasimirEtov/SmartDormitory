using SmartDormitory.Services.Models.MeasureTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.App.Areas.Administration.Models.MeasureType
{
	public class MeasureTypesListViewModel
	{
		public IEnumerable<MeasureTypeServiceModel> MeasureTypes { get; set; }
	}
}
