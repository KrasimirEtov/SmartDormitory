using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartDormitory.App.Models.Sensor
{
    public class CreateUpdateSensorViewModel : IValidatableObject
    {
        public string SensorId { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Sensor name should be between 3 and 30 symbols.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string IcbSensorId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Sensor description should be between 3 and 30 symbols.", MinimumLength = 3)]
        public string Description { get; set; }

        [Required]
        public int PollingInterval { get; set; }

        public bool IsPublic { get; set; }

        public bool AlarmOn { get; set; }

        public float MinRangeValue { get; set; }

        public float MaxRangeValue { get; set; }

        public double Longtitude { get; set; }

        public double Latitude { get; set; }

        public bool IsSwitch { get; set; }

        public float ApiMinRangeValue { get; set; }

        public float ApiMaxRangeValue { get; set; }

        public int ApiPollingInterval { get; set; }

        public bool SwitchOn { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (this.MinRangeValue >= this.MaxRangeValue)
            {
                results.Add(new ValidationResult("Max range should be bigger than min range value!"));
            }

            return results;
        }
    }
}
