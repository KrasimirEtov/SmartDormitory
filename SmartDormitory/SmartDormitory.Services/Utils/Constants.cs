namespace SmartDormitory.Services.Utils
{
    public static partial class Constants
    {
        public static class IcbApi
        {
            public const string BaseUrl = "http://telerikacademy.icb.bg/";
            public const string IcbSensorPostfix = "api/sensor/";
            public const string AllSensorsPostfix = "all";
            public const string AuthorizationHeaderKey = "auth-token";
            public const string AuthorizationHeaderValue = "8e4c46fe-5e1d-4382-b7fc-19541f7bf3b0";
            public const string AcceptHeaderKey = "Accept";
            public const string AcceptHeaderValue = "application/json";
        }

        public static class ValidatorConstants
        {
            public const string NullExceptionMessage = "Parameter {0} cannot be null!";
            public const string GuidExceptionMessage = "Parameter {0} is not a valid GUID!";
            public const string IntBiggerThanMessage = "Parameter {0} must be bigger than {1}!";
		}
    }
}
