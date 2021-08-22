using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CoreFramework.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool HasValueFor(this Dictionary<string, string> theDataRow, string column) =>
            theDataRow.ContainsKey(column) &&
            !string.IsNullOrWhiteSpace(theDataRow[column]);

        public static bool HasValuesFor(this Dictionary<string, string> theDataRow, params string[] columns) =>
            columns.All(theDataRow.HasValueFor);

        public static IDictionary<string, string> ToDictionary(this IDictionary dictionary) =>
            dictionary.Keys.Cast<object>().ToDictionary(k => k.ToString(), v => dictionary[v].ToString(), StringComparer.InvariantCultureIgnoreCase);

    }
}
