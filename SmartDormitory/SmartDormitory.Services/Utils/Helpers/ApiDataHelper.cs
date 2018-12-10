using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmartDormitory.Services.Utils.Helpers
{
    public static class ApiDataHelper
    {
        public static (int MinRange, int MaxRange) GetMinAndMaxRange(string description)
        {
            if (description.Any(char.IsDigit))
            {
                var numbers = Regex.Matches(description, @"\d+");

                int minRange = int.Parse(numbers[0].Value);
                int maxRange = int.Parse(numbers[1].Value);

                return (MinRange: minRange, MaxRange: maxRange);
            }
            //              false    true
            return (MinRange: 0, MaxRange: 1);
        }

        public static float GetLastValue(string lastValue)
        {
            bool isParsable = float.TryParse(lastValue, out float value);

            if (!isParsable)
            {
                return lastValue.Equals("true") ? 1 : lastValue.Equals("false") ? 0
                                : throw new InvalidOperationException("Invalid last value response");
            }

            return value;
        }
    }
}
