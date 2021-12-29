namespace eu.iamia.Util.Extensions
{
    using System;

    // ReSharper disable once IdentifierTypo
    public static class CompareableExtension
    {
        public static bool IsWithinRange<T>(this T value, T lowerInclusive, T upperInclusive) where T : IComparable<T>
        {
            return
                value.CompareTo(lowerInclusive) >= 0
                &&
                value.CompareTo(upperInclusive) <= 0;
        }

        public static bool IsOutsideRange<T>(this T value, T lowerExclusive, T upperExclusive) where T : IComparable<T>
        {
            return
                value.CompareTo(lowerExclusive) < 0
                ||
                value.CompareTo(upperExclusive) > 0;
        }
    }
}
