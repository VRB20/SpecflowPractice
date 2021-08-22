namespace CoreFramework.Extensions
{
    public static class ConversionExtensions
    {
        public static double ToDoubleSafe(this string theString)
        {
            if (string.IsNullOrWhiteSpace(theString)) return default;

            return double.TryParse(theString, out var result)
                ? result
                : default;
        }

        public static int ToIntSafe(this string theString)
        {
            if (string.IsNullOrWhiteSpace(theString)) return default;

            return int.TryParse(theString, out var result)
                ? result
                : default;
        }

        public static long ToLongSafe(this string theString)
        {
            if (string.IsNullOrWhiteSpace(theString)) return default;

            return long.TryParse(theString, out var result)
                ? result
                : default;
        }

        public static long ToLongSafe(this int? theInt) => theInt ?? default(long);

        public static decimal? ToNullableDecimalSafely(this string theString)
        {
            if (string.IsNullOrWhiteSpace(theString)) return default;

            return decimal.TryParse(theString, out var result)
                ? result
                : default;
        }

        public static double? ToNullableDoubleSafe(this string theString)
        {
            if (string.IsNullOrWhiteSpace(theString)) return default;

            return double.TryParse(theString, out var result)
                ? result
                : default;
        }

        public static int? ToNullableIntSafe(this string theString, bool emptyToNullConversion = true)
        {
            if (emptyToNullConversion && string.IsNullOrEmpty(theString)) return null;
            if (string.IsNullOrWhiteSpace(theString)) return default;

            return int.TryParse(theString, out var result)
                ? result
                : default;
        }

        public static long? ToNullableLongSafe(this string theString)
        {
            if (string.IsNullOrWhiteSpace(theString)) return default;

            return long.TryParse(theString, out var result)
                ? result
                : default;
        }
    }
}
