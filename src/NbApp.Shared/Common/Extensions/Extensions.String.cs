using System;
// ReSharper disable once CheckNamespace
namespace Common
{
    public static partial class Extensions
    {
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool MyContains(this string value, string search)
        {
            if (value == null)
            {
                return false;
            }
            return value.Contains(search, StringComparison.OrdinalIgnoreCase);
        }

        public static bool MyEquals(this string value, string search)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.IsNullOrWhiteSpace(search);
            }
            return value.Equals(search, StringComparison.OrdinalIgnoreCase);
        }
    }
}
