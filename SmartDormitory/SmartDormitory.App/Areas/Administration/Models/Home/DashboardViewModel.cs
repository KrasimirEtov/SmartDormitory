using SmartDormitory.Services.Models.Sensors;
using System.Collections.Generic;

namespace SmartDormitory.App.Areas.Administration.Models.Home
{
    public class DashboardViewModel
    {
        public int UsersCount { get; set; }
        public int SensorsCount { get; set; }
        public int MeasureTypesCount { get; set; }
    }
}
