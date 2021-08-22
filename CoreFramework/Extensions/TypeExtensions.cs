using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreFramework.Extensions
{
    public static class TypeExtensions
    {
        public static List<PropertyInfo> GetPropertiesThatStartWith(this Type type, string startsWithFilter) => type
            .GetProperties()
            .Where(propertyInfo => propertyInfo.Name.StartsWith(startsWithFilter, StringComparison.InvariantCultureIgnoreCase))
            .ToList();

        public static object GetPropertyValueByName<T>(this T theObject, string propertyName)
        {
            var matchingPropertiesOneExpected = theObject.GetType()
                .GetProperties()
                .Where(PropertyInfo => PropertyInfo.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (!matchingPropertiesOneExpected.Any()) throw new Exception($"Unable to find {propertyName} in the following json{Environment.NewLine}{theObject.ToJson()}");
            if (matchingPropertiesOneExpected.Count() > 1) throw new Exception();

            return matchingPropertiesOneExpected
                .Single()
                .GetValue(theObject);
        }
        public static Type GetUnderlyingType<T>(this T value) => Nullable.GetUnderlyingType(value.GetType());

        public static object GetUnderlyingValueIfNullable(this object theObject)
        {
            if (!theObject.GetType().ToString().Contains("Nullable"))
                return theObject;

            return theObject.GetType().GetProperties()
                .Single(x => x.Name.Equals("Value", StringComparison.InvariantCultureIgnoreCase))
                .GetValue(theObject);
        }

        public static bool IsNullable<T>(this T value) => Nullable.GetUnderlyingType(value.GetType()) != null;
    }
}
