using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreFramework.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current)) yield return current;
        }

        public static IEnumerable<Tsource> FromHierarchy<Tsource>(
            this Tsource source,
            Func<Tsource, Tsource> nextItem)
            where Tsource : class => FromHierarchy(source, nextItem, s => s != null);

        public static bool In<T>(this T source, IEnumerable<T> list) => list.Contains(source);
        public static bool In<T>(this T source, params T[] list) => list.ToList().Contains(source);
    }
}
