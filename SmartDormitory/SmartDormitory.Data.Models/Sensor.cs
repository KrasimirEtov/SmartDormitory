using Microsoft.EntityFrameworkCore;
using SmartDormitory.Data.Models.Abstract;
using System;
using System.ComponentModel.DataAnnotations;
using static SmartDormitory.Data.Models.Utils.Constants;

namespace SmartDormitory.Data.Models
{
    public class Sensor : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public string IcbSensorId { get; set; }
        public IcbSensor IcbSensor { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = DomainConstants.PollingIntervalMessage)]
        public int PollingInterval { get; set; }

        public bool IsPublic { get; set; }

        public bool AlarmOn { get; set; }

        public bool SwitchOn { get; set; }

        public float MinRangeValue { get; set; }

        public float MaxRangeValue { get; set; }

        public Coordinates Coordinates { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastUpdateOn { get; set; }

        public float CurrentValue { get; set; }
    }

    // EF Core allows you to model entity types that can only ever appear on navigation properties of other entity types. These are called owned entity types. The entity containing an owned entity type is its owner.
    [Owned]
    public class Coordinates
    {
        [Range(DomainConstants.LongitudeMinValue, DomainConstants.LongitudeMaxValue,
            ErrorMessage = DomainConstants.LongitudeErrorMessage)]
        public double Longitude { get; set; }

        [Range(DomainConstants.LatitudeMinValue, DomainConstants.LatitudeMinValue,
            ErrorMessage = DomainConstants.LatitudeErrorMessage)]
        public double Latitude { get; set; }
    }
}
