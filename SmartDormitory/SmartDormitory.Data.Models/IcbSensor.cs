using SmartDormitory.Data.Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static SmartDormitory.Data.Models.Utils.Constants.DomainConstants;

namespace SmartDormitory.Data.Models
{
    public class IcbSensor : BaseEntity
    {
        [Required]
        public float MinRangeValue { get; set; }

        [Required]
        public float MaxRangeValue { get; set; }

        [Required]
        public string Tag { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = PollingIntervalMessage)]
        public int PollingInterval { get; set; }

        public MeasureType MeasureType { get; set; }
        public string MeasureTypeId { get; set; }

        public ICollection<Sensor> Sensors { get; set; } = new HashSet<Sensor>();

        [DataType(DataType.DateTime)]
        public DateTime LastUpdateOn { get; set; }

        public float CurrentValue { get; set; }
    }
}
