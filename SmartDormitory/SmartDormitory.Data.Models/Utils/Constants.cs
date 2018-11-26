namespace SmartDormitory.Data.Models.Utils
{
    public static partial class Constants
    {
        public static class DomainConstants
        {
            public const int LatitudeMinValue = -90;
            public const int LatitudeMaxValue = 90;
            public const int LongitudeMinValue = -180;
            public const int LongitudeMaxValue = 180;

            public const string LongitudeErrorMessage = "Longitude must be between -180 and 180 !";
            public const string LatitudeErrorMessage = "Longitude must be between -90 and 90 !";

            public const string UserPollingIntervalErrorMessage = "Polling interval cannot be negative!";
        }
    }
}
