using CoreFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreFramework.ObjectGraphBatchValidation.Models
{
    public static class BatchModelExtensions
    {
        public static Batch FilterBatchByTheFollowingKeys(this Batch theBatch, string keyToFilter) => theBatch
            .Select(batchItem => batchItem
                 .Where(pair => !pair.Key.Equals(keyToFilter, StringComparison.InvariantCultureIgnoreCase))
                 .ToDictionary(y => y.Key, y => y.Value)
                 .ToBatchItem())
            .ToBatch();

        public static Batch FilterBatchByTheFollowingKeys(this Batch theBatch, params string[] keysToFilter)
        {
            foreach (var key in keysToFilter) theBatch = theBatch.FilterBatchByTheFollowingKeys(key);

            return theBatch;
        }

        public static List<int> GetValueAsIntList(this BatchItem item, string field, char delimiter = ',') => item
            .GetValueAsList(field, delimiter)
            .Select(x => x.ToIntSafe())
            .ToList();

        public static List<string> GetValueAsList(this BatchItem item, string field, char delimiter = ',')
        {
            if (!item.HasValueFor(field)) return new List<string>();

            return item[field]
                .Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        public static List<long> GetValueAsLongList(this BatchItem item, string field, char delimiter = ',') => item
            .GetValueAsList(field, delimiter)
            .Select(x => x.ToLongSafe())
            .ToList();

        public static string GetValueOrDefault(this BatchItem item, string field) =>
            item.HasValueFor(field) ? item[field] : default;

        public static string GetValueOrDefault2(this BatchItem batchItem, string key) => batchItem.GetValueOrDefault(key);

        public static string GetValueOrEmpty(this BatchItem item, string field) =>
            item.HasValueFor(field) ? item[field] : string.Empty;

        public static Batch ToBatch(this IEnumerable<Dictionary<string, string>> batchItems) => new Batch(batchItems.Select(ToBatchItem));
        public static BatchItem ToBatchItem(this Dictionary<string, string> theDictionary) => new BatchItem(theDictionary);

        public static Batch ToEvenBatch(this IEnumerable<Dictionary<string, string>> batch)
        {
            var allKeys = batch
                .SelectMany(batchItem => batchItem.Keys)
                .Distinct();

            return batch
                .Select(batchItem => allKeys
                   .ToDictionary(
                        key => key,
                        key => batchItem.ContainsKey(key) ? batchItem[key] : null
                        )).ToBatch();
        }
    }
}
