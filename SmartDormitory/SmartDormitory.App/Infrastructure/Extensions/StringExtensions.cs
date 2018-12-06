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
    }
}
