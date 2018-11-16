using SmartDormitory.Data.Models.Abstract;
using System.Collections.Generic;

namespace SmartDormitory.Data.Models
{
    public class ApiSensor : BaseEntity
    {
        public int MinRangeValue { get; set; }

        public int MaxRangeValue { get; set; }

        public string Tag { get; set; }

        public string Description { get; set; }

        public int MinPollingInterval { get; set; }

        public string MeasureType { get; set; }

        public string LatestResultId { get; set; }
        public LatestApiSensorResult LatestResult { get; set; }

        public string ApiFetchUrl { get; set; }

        public ICollection<Sensor> Sensors { get; set; } = new HashSet<Sensor>();
    }
}
