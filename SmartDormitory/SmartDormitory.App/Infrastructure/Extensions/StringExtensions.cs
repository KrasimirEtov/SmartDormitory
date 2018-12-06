using System.Text.RegularExpressions;

namespace SmartDormitory.App.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string SplitTag(this string source)
        {
            var results = Regex.Split(source, @"(?<!^)(?=[A-Z0-9])");

            return string.Join(" ", results);
        }

        public static string GetImagePathByMeasureUnit(this string measureUnit)
        {
            string path = string.Empty;

            switch (measureUnit)
            {
                case "°C": path = "/images/sensortypes/temperature.jpg"; break;
                case "W": path = "/images/sensortypes/electric.jpg"; break;
                case "%": path = "/images/sensortypes/humidity.jpg"; break;
                case "(true/false)": path = "/images/sensortypes/switch.jpg"; break;
                case "dB": path = "/images/sensortypes/noise.jpg"; break;
                default:
                    break;
            }

            return path;
        }
    }
}
