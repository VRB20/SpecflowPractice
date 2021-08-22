using CoreFramework.ObjectGraphBatchValidation.Models;
using FluentAssertions;
using System;

namespace CoreFramework.ObjectGraphBatchValidation
{
    public static class ValidationExtensions
    {
        public static void PerformDecimalComparisonTo(this decimal? actual, decimal? expected)
        {
            var result = PerformDecimalRounding(actual, expected);
            result.Actual.Should().Be(result.Expected);
        }

        public static ValidationErrors PerformDecimalComparisonTo(this decimal actual, decimal? expected)
        {
            var (actualRounded, expectedRounded) = PerformDecimalRounding(actual, expected);

            return actualRounded == expectedRounded
                ? ValidationErrors.Empty
                : $"Actual: '{actualRounded}' does not equal Expected: '{expectedRounded}'";

        }

        public static (decimal? Actual, decimal? Expected) PerformDecimalRounding(this decimal? actual, decimal? expected)
        {
            if (!actual.HasValue || !expected.HasValue)
                return (actual, expected);

            int actualDecimalPlaces = BitConverter.GetBytes(decimal.GetBits(actual.Value)[3])[2];
            int expectedDecimalPlaces = BitConverter.GetBytes(decimal.GetBits(expected.Value)[3])[2];

            if (actualDecimalPlaces < expectedDecimalPlaces)
                expected = decimal.Round(expected.Value, actualDecimalPlaces);
            else
                actual = decimal.Round(actual.Value, expectedDecimalPlaces);

            return (actual, expected);
        }

        public static (decimal Actual, decimal? Expected) PerformDecimalRounding(this decimal actual, decimal? expected)
        {
            if (!expected.HasValue) return (actual, null);

            int actualDecimalPlaces = BitConverter.GetBytes(decimal.GetBits(actual)[3])[2];
            int expectedDecimalPlaces = BitConverter.GetBytes(decimal.GetBits(expected.Value)[3])[2];

            if (actualDecimalPlaces < expectedDecimalPlaces)
                expected = decimal.Round(expected.Value, actualDecimalPlaces);
            else
                actual = decimal.Round(actual, expectedDecimalPlaces);

            return (actual, expected);
        }

        public static decimal? ToDecimal(this string theString, decimal defaultValue = decimal.Zero)
        {
            if (string.IsNullOrWhiteSpace(theString))
                return defaultValue;

            if (theString.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                return null;

            return decimal.TryParse(theString, out var theResult) ? theResult : defaultValue;
        }

        public static string WithNullConversion(this string theString)
        {
            if (theString == null || theString.Equals("null", StringComparison.InvariantCultureIgnoreCase)) theString = null;
            return theString;
        }
    }
}
