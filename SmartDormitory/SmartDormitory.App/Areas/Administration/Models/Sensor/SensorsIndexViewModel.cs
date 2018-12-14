using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SmartDormitory.App.Areas.Administration.Models.Sensor
{
    public class SensorsIndexViewModel
    {
        public string MeasureTypeId { get; set; } = "all";

        [Display(Name = "Sensor type")]
        public SelectList MeasureTypes { get; set; }

        [Display(Name = "Privacy")]
        public int IsPublic { get; set; } = -1;

        [Display(Name = "Alarms")]
        public int AlarmSet { get; set; } = -1;

        public SensorPartialTableViewModel PartialModel { get; set; }
    }
}
