
using CoreFramework.Extensions;
using CoreFramework.ObjectGraphBatchValidation.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CoreFramework.ObjectGraphBatchValidation
{
    public static class DictionaryExtensions
    {
        public static Batch FilterBy(this Batch batch, string startsWithFilter)
        {
            var filteredKeys = batch[0].Keys
                .Where(x => x.StartsWith(startsWithFilter))
                .ToList();

            return batch
                .Select(dictionary => dictionary
                    .Where(item => item.Key.In(filteredKeys))
                    .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                    .ToDictionary(x => x.Key, x => x.Value))
                .RemoveEmptyRows();
        }

        public static List<BatchItem> FilterByKey<T>(this List<T> theList, string startsWithFilter)
            where T : BatchItem =>
            theList.Select(csvLine => csvLine.Keys
                        .Where(key => key.StartsWith(startsWithFilter, StringComparison.InvariantCultureIgnoreCase))
                        .ToDictionary(
                            key => key.Replace(startsWithFilter, string.Empty),
                            key => csvLine[key])
                        .ToValidationBatchItem()
                ).ToList();

        public static Batch RemoveEmptyColumns(this IEnumerable<Dictionary<string, string>> theListOfDictionaries) => theListOfDictionaries
            .Select(RemoveEmptyColumns)
            .ToBatch();

        public static BatchItem RemoveEmptyColumns(this Dictionary<string, string> theDictionary) => theDictionary.Keys
            .Where(key => !string.IsNullOrWhiteSpace(theDictionary[key]))
            .ToDictionary(x => x, key => theDictionary[key])
            .ToBatchItem();

        public static Batch RemoveEmptyRows(this IEnumerable<Dictionary<string, string>> theListOfDictionaries) =>
            theListOfDictionaries
                .Where(x => !x.Values.All(string.IsNullOrWhiteSpace))
            .ToBatch();

        public static (Batch ConstructionBatch, Batch validationBatch)
            SeparateConstructionFromValidationBatches(this Batch batch)
        {
            var validationBatch = batch
                .Select(batchItem => batchItem.Keys
                .Where(key => key.StartsWith("Response.", StringComparison.InvariantCultureIgnoreCase))
                .ToDictionary(
                    x => x.WithoutParentProperty("Response"),
                    x => batchItem[x]).ToBatchItem()).ToBatch();

            var constructionBatch = batch
                .Select(batchItem =>
                {
                    var keysToExclude = batchItem.Keys
                    .Where(key => key.StartsWith("Response.", StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

                    var keysToKeep = batchItem.Keys.Except(keysToExclude).ToList();

                    return keysToKeep.ToDictionary(
                    key => key,
                    key => batchItem[key])
                    .ToBatchItem();
                }).ToBatch();

            return (constructionBatch, validationBatch);

        }

        public static (Batch DirectValidationBatch, List<Dictionary<string, BatchItem>> ChildValidationBatch) SeparateDirectPropertyAndChildValidations(this List<Dictionary<string, string>> batches)
        {
            var results = batches.Select(SeparateDirectPropertyAndChildValidations).ToList();

            return (
            results.Select(x => x.DirectPropertyValidationBatch).ToList().ToBatch(),
            results.Select(x => x.ChildPropertyValidationBatchs).ToList());
        }

        public static (BatchItem DirectPropertyValidationBatch,
            Dictionary<string, BatchItem> ChildPropertyValidationBatchs,
            List<KeyValuePair<string, List<Batch>>> NewChildPropertyValidationBatches)
            SeparateDirectPropertyAndChildValidations(this Dictionary<string, string> batch)
        {
            var childPropertyKeys = batch.Keys.Where(key => key.Contains(".")).ToList();
            var directKeys = batch.Keys.Except(childPropertyKeys).ToList();

            var childPropertyValidationBatch = childPropertyKeys.ToDictionary(x => x, x => batch[x]);
            var directPropertyValidationBatch = directKeys.ToDictionary(x => x, x => batch[x]);

            var childPropertyNames = childPropertyKeys
                    .Select(key => key.Split('.')[0])
                    .Distinct()
                    .ToList();

            var childPropertyValidationBatches = childPropertyNames
                .Select(childPropertyKey =>
                {
                    var childKeysOfInterest = childPropertyValidationBatch.Keys.Where(key => key.StartsWith(childPropertyKey));

                    var theBatch = batch.Keys
                        .Where(key => key.In(childKeysOfInterest))
                        .ToDictionary(key => key, key => batch[key]);
                    return new KeyValuePair<string, Dictionary<string, string>>(childPropertyKey, theBatch);
                })
                .ToDictionary(x => x.Key, x => x.Value.ToBatchItem());

            var newChildPropertyValidationBatches = childPropertyNames
                .Select(childPropertyKey =>
                {
                    var childKeysOfInterest = childPropertyValidationBatch.Keys.Where(key => key.StartsWith(childPropertyKey));

                    var theBatch = batch.Keys
                        .Where(key => key.In(childKeysOfInterest))
                        .ToDictionary(key => key, key => batch[key]);

                    var valueBatch = theBatch
                            .Select(batchItem =>
                            {
                                return (
                                    batchItem.Key,
                                    Values: batch[batchItem.Key].Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries));
                            }).ToList();

                    var maxValuesArray = valueBatch.Max(x => x.Values.Length);
                    var batches = Enumerable.Range(0, maxValuesArray)
                        .Select(index => valueBatch
                                .Select(item => new BatchItem(item.Key, item.Values.Length - 1 >= index ? item.Values[index] : default))
                                .RemoveEmptyColumns()
                                .RemoveEmptyRows()
                                .ToBatch())
                               .ToList();

                    return new KeyValuePair<string, List<Batch>>(childPropertyKey, batches);
                })
                .ToDictionary(x => x.Key, x => x.Value)
                .ToList();

            return (
                DirectPropertyValidationBatch: directPropertyValidationBatch.ToBatchItem(),
                ChildPropertyValidationBatchs: childPropertyValidationBatches,
                NewChildPropertyValidationBatches: newChildPropertyValidationBatches);
        }

        public static Dictionary<string, string> ToChildBatch(this Dictionary<string, string> dictionary, string propertyName) =>
            dictionary.ToDictionary(x => x.Key.WithoutParentProperty(propertyName),
                                    x => x.Value);

        public static ValidationErrors ValidateExistsInCollection(this IEnumerable collection, (string Key, String Value) validationPair)
        {
            var anyPass = false;

            foreach (var item in collection)
                try
                {
                    item.PerformKeyValuePairValidation(validationPair);
                    anyPass = true;
                    break;
                }
                catch (Exception)
                {

                }

            return anyPass
                ? ValidationErrors.Empty
                : $@"Could not find {validationPair.Key}-{validationPair.Value} in collection {collection.ToJson()}";
        }

        public static string WithoutParentProperty(this string originalString, string parentName) => originalString
            .Replace($"{parentName}.", string.Empty);
    }
}
