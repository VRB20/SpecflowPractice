using System;
using System.Diagnostics;

namespace CoreFramework.Extensions
{
    public static class LogExtensions
    {
        public static string Log(this string theMessage, string optionalPrefix = null)
        {
            var logMessage = $"{optionalPrefix}{theMessage}";
            Debug.WriteLine(logMessage);
            Console.WriteLine(logMessage);

            return theMessage;
        }

        public static T Log<T>(this T theObject, string optionalPrefix = null)
        {
            var logMessage = $"{optionalPrefix}{theObject.ToJson()}";
            Debug.WriteLine(logMessage);
            Console.WriteLine(logMessage);

            return theObject;
        }
    }
}
