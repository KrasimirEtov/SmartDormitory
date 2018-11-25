using SmartDormitory.Data.Models.Abstract;
using System.Collections.Generic;

namespace SmartDormitory.Data.Models
{
    public class MeasureType : BaseEntity
    {
        public string MeasureUnit { get; set; }

        // description for ui: "Temperature", "Noise"... etc
        public string SuitableSensorType { get; set; }

        public ICollection<IcbSensor> IcbSensors { get; set; } = new HashSet<IcbSensor>();
    }
}
