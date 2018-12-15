using System;
using static SmartDormitory.Services.Utils.Constants;

namespace SmartDormitory.Services.Utils
{
    public static class Validator
    {
        public static void ValidateGuid(string value)
        {
            if (!Guid.TryParse(value, out Guid temp))
            {
                throw new ArgumentException(string.Format(ValidatorConstants.GuidExceptionMessage, nameof(value)));
            }
        }

        public static void ValidateNull(Object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(string.Format(ValidatorConstants.NullExceptionMessage, nameof(value)));
            }
        }
    }
}
