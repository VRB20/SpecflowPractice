using CoreFramework.Extensions;
using CoreFramework.ObjectGraphBatchValidation.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CoreFramework.ObjectGraphBatchValidation
{
    public static class ObjectValidationExtensions
    {
        public static ValidationErrors PerformResponseBatchValidation<T>(this T objectToValidate, Batch batch) =>
            objectToValidate is IEnumerable enumerable
                ? enumerable.PerformBatchValidation(batch)
                : objectToValidate.PerformIndividualBatchValidation(batch);

        public static ValidationErrors PerformBatchValidation<T>(this T objectToValidate, Batch batch) =>
            objectToValidate is IEnumerable enumerable
                ? enumerable.PerformBatchValidation(batch)
                : batch.SelectMany(x => objectToValidate.PerformBatchValidation(x)).ToList().ToValidationErrors();

        public static ValidationErrors PerformBatchValidation(this IEnumerable objectToValidate, Batch batch) =>
            batch.SelectMany(objectToValidate.PerformBatchValidation).ToValidationErrors();

        public static ValidationErrors PerformBatchValidation(this IEnumerable objectToValidate, BatchItem batchItem) =>
            ValidationErrors.From(
                objectToValidate.Cast<object>()
                    .SelectMany(item => PerformBatchValidation(item, batchItem))
                    .ToList());

        public static ValidationErrors PerformBatchValidation<T>(this T objectToValidate, BatchItem batchItem)
        {
            if (objectToValidate is IEnumerable enumerable)
            {
                var collectionEnumerationResult = enumerable.Cast<object>()
                    .Select(item => item.PerformBatchValidation(batchItem))
                    .ToList();

                var theBatchFoundInCollection = collectionEnumerationResult.Any(x => !x.Any());

                return theBatchFoundInCollection
                    ? ValidationErrors.Empty
                    : string.Join(Environment.NewLine, collectionEnumerationResult.SelectMany(x => x));
            }

            //TODO: remove unused return value

            var (directPropertyValidationBatch, childPropertyValidationBatch, newChildPropertyValidationBatches) = batchItem.SeparateDirectPropertyAndChildValidations();
            var performPropertyBatchValidationErrors = ValidationErrors.Empty;
            if (directPropertyValidationBatch.Any())
                performPropertyBatchValidationErrors = PerformPropertyBatchValidation(objectToValidate, directPropertyValidationBatch);

            var childValidationBatchToProcess = newChildPropertyValidationBatches
                .SelectMany(newChildPropertyValidationBatch =>
                {
                    var list = newChildPropertyValidationBatch.Value
                        .Select(batch =>
                        {
                            var valueTuples = batch
                                .SelectMany(item => item.Keys
                                    .Select(key =>
                                    {
                                        var parentKey = key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
                                        return (Key: parentKey,
                                                BatchItem: new BatchItem(key.WithoutParentProperty(parentKey), item[key]));
                                    })
                                    .Select(item2 =>
                                    {
                                        var propertyValueByName = objectToValidate.GetPropertyValueByName(item2.Key);
                                        var debug = propertyValueByName == null;
                                        return (
                                                TheObject: propertyValueByName, item2.BatchItem);
                                    })
                                    .ToList())
                                .ToList();

                            var theObjectToValidate = valueTuples.First().TheObject;
                            var theNewBatch = valueTuples.Select(x => x.BatchItem).ToBatch();
                            return (TheObject: theObjectToValidate, Batch: theNewBatch);
                        })
                        .ToList();
                    return list;
                })
                .Select(x => x)
                .ToList();

            var childValidationResults = childValidationBatchToProcess
                .Select(batch =>
                {
                    ValidationErrors results;
                    switch (batch.TheObject)
                    {
                        case IEnumerable collection:
                            var collectionValidationResults = collection.Cast<object>().Select(theItem => theItem.PerformBatchValidation(batch.Batch)).ToList();
                            if (collectionValidationResults.Any(errors => !errors.Any()))
                                results = ValidationErrors.Empty;
                            else
                                results = collectionValidationResults.SelectMany(e => e).ToValidationErrors();
                            break;
                        default:
                            results = batch.TheObject.PerformBatchValidation(batch.Batch);
                            break;
                    }
                    return results;
                }).ToList().ToValidationErrors();

            return ValidationErrors.From(childValidationResults.Union(performPropertyBatchValidationErrors));
        }

        public static ValidationErrors PerformIndividualBatchValidation<T>(this T objectToValidate, Batch batch)
        {
            JObject jResponseToValidate = JObject.Parse(JsonConvert.SerializeObject(objectToValidate));

            var directPropertiesBatch = batch.First().Where(item => !item.Key.Contains(".")).ToDictionary(x => x.Key, x => x.Value);
            var directPropertiesValidationResults = directPropertiesBatch.Select(item =>
            {
                if (!jResponseToValidate.ContainsKey($"{item.Key}"))
                    return $"Unable to find property{item.Key}";

                if (jResponseToValidate.SelectToken($"{item.Key}").ToString() != item.Value)
                    return $"Unable to find property{item.Key} with value {item.Value}";
                else
                    return ValidationErrors.Empty;
            }).ToList().ToValidationErrors();

            var childPropertiesBatch = batch.Select(item => item.Except(directPropertiesBatch).ToDictionary(x => x.Key, x => x.Value)).ToBatch();
            var childValidationResults = objectToValidate.PerformChildValidation(childPropertiesBatch).ToList().ToValidationErrors();

            return directPropertiesValidationResults.Concat(childValidationResults).ToList().ToValidationErrors();
        }

        public static ValidationErrors PerformChildValidation<T>(this T objectToValidate, Batch batch)
        {
            bool isArray = false;

            var batchItemsConsolidatedResult = batch.Select(items =>
            {
                //Group the child items based on their parent
                var groupItems = items.GroupBy(x => x.Key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0])
                                .Distinct()
                                .ToList();

                //For every grouped child batch
                var validationResults = groupItems.Select(item =>
                {
                    var directPropertyValidationResults = ValidationErrors.Empty;
                    var childPropertyValidationResults = ValidationErrors.Empty;
                    var parentKey = item.Key;
                    IEnumerable<JToken> responseToValidate = null;

                    dynamic resObjectToValidate;

                    switch (objectToValidate)
                    {
                        case JArray response:
                            resObjectToValidate = JArray.Parse(JsonConvert.SerializeObject(response));
                            isArray = true;
                            break;

                        case JObject response:
                            resObjectToValidate = JObject.Parse(JsonConvert.SerializeObject(response));
                            break;

                        default:
                            resObjectToValidate = JObject.Parse(JsonConvert.SerializeObject(objectToValidate));
                            break;
                    };

                    var directProperties = item.Where(x => !x.Key.WithoutParentProperty(parentKey).Contains(".")).ToDictionary(x => x.Key, x => x.Value);
                    var childProperties = item.Except(directProperties).ToDictionary(x => x.Key, x => x.Value).ToBatchItem();


                    if (directProperties.Any())
                    {
                        var directPropertiesBatch = directProperties
                            .ToDictionary(d => d.Key.WithoutParentProperty(parentKey), d => d.Value.WithoutParentProperty(parentKey).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());

                        var o = directPropertiesBatch.First().Value.Select((identifier, index) =>
                        {
                            var directProp = directPropertiesBatch.Select(c => c).ToDictionary(d => d.Key, d => d.Value[index]);
                            if (isArray)
                            {
                                var respo = (JEnumerable<JToken>)resObjectToValidate.Children();
                                responseToValidate = respo.SelectMany(x =>
                                {
                                    var node = x.SelectToken($".{parentKey}")
                                        .Where(y => y?.Value<string>(directPropertiesBatch.First().Key) == identifier);
                                    return node;
                                });
                            }
                            else
                            {
                                responseToValidate = resObjectToValidate.SelectToken($"$.{parentKey}");
                                responseToValidate = responseToValidate.Where(c => c?.Value<string>(directPropertiesBatch.First().Key) == identifier);
                            }

                            if (!responseToValidate.Any())
                                return $"\n Unable to find identifier property {directPropertiesBatch.First().Key} with value {directPropertiesBatch.First().Value}";

                            directPropertyValidationResults = directProp.Select(prop =>
                            {
                                var results = responseToValidate.Select(token =>
                                {
                                    var responseValue = token?.Value<dynamic>($"{prop.Key}").Value;
                                    var validationValue = prop.Value.Trim();

                                    if (responseValue is double && prop.Value.Contains("."))
                                    {
                                        var valWithTrimmedTrailingZeroes = prop.Value.TrimEnd(new[] { '0' });
                                        validationValue = valWithTrimmedTrailingZeroes.Substring(valWithTrimmedTrailingZeroes.Length - 1).Contains(".") ? valWithTrimmedTrailingZeroes.Replace(".", string.Empty) : valWithTrimmedTrailingZeroes;
                                    }

                                    if (validationValue.ToLower() == "null" ? !string.IsNullOrEmpty(token?.Value<string>($"{ prop.Key}")) : token?.Value<string>($"{prop.Key}") != validationValue)
                                        return $"\n For {directPropertiesBatch.First().Key} : {identifier} - " +
                                        $"Unable to find property {prop.Key} with value {prop.Value} " +
                                        $"\n Response is {JsonConvert.SerializeObject(token)}";
                                    else
                                        return ValidationErrors.Empty;
                                }).ToList().ToValidationErrors();
                                return results;
                            }).ToList().ToValidationErrors();

                            return directPropertyValidationResults;
                        }).ToList().ToValidationErrors();
                    }

                    if (childProperties.Any())
                    {
                        var childPropertyValidationBatch = new Batch(new List<BatchItem> { childProperties.ToDictionary(v => v.Key.WithoutParentProperty(parentKey), v => v.Value).ToBatchItem() });
                        var re = responseToValidate == null ? resObjectToValidate.SelectToken($"$.{parentKey}").ToString()
                            : responseToValidate.First().ToString();
                        var jre = (object)JsonConvert.DeserializeObject(re.ToString());
                        childPropertyValidationResults = jre.PerformChildValidation(childPropertyValidationBatch).ToList().ToValidationErrors();
                    }

                    return ValidationErrors.From(childPropertyValidationResults.Union(directPropertyValidationResults));

                }).ToList();

                return validationResults.ToValidationErrors();
            }).ToList().ToValidationErrors();
            return batchItemsConsolidatedResult;
        }
        public static ValidationErrors PerformChildPropertyBatchValidation<T>(T objectToValidate, BatchItem batchItem, Dictionary<string, BatchItem> theBatch, Dictionary<string, BatchItem> childPropertyValidationBatch)
        {
            return theBatch.Keys
                .Select(key => key.Split('.')[0])
                .Distinct()
                .Select(childPropertyKey =>
                {
                    var childKey = childPropertyValidationBatch.Keys.Single(key => key.StartsWith(childPropertyKey));

                    var batch1 = batchItem.Keys
                        .Where(key => key.StartsWith(childKey, StringComparison.InvariantCultureIgnoreCase))
                        .ToDictionary(key => key.WithoutParentProperty(childKey), key => batchItem[key])
                        .ToValidationBatchItem();

                    return new KeyValuePair<string, BatchItem>(childPropertyKey, batch1);
                }).ToDictionary(x => x.Key, x => x.Value)
                .SelectMany(batch =>
                {
                    var childPropertyName = batch.Key;

                    if (objectToValidate is IEnumerable enumerableProperty)
                        return ValidationErrors.From(enumerableProperty.Cast<object>()
                            .SelectMany(item =>
                            {
                                var childProperty1 = item.GetPropertyValueByName(childPropertyName);
                                var batchItem1 = batch.Value
                                    .ToChildBatch(childPropertyName)
                                    .ToDictionary(x => x.Key, x => x.Value)
                                    .ToValidationBatchItem();

                                return childProperty1.PerformBatchValidation(batchItem1);
                            }).ToList());

                    var childProperty = objectToValidate.GetPropertyValueByName(childPropertyName);

                    return childProperty.PerformBatchValidation(batch.Value.ToChildBatch(childPropertyName).ToDictionary(x => x.Key, x => x.Value).ToValidationBatchItem());
                }).ToList()
                .ToValidationErrors();
        }
        public static ValidationErrors PerformKeyValuePairValidation<T>(this T objectToValidate, KeyValuePair<string, string> validationPair)
        {
            switch (objectToValidate)
            {
                case IEnumerable enumerable:
                    return enumerable.ValidateExistsInCollection((validationPair.Key, validationPair.Value));

                default:
                    return PerformKeyValuePairValidation(objectToValidate, (validationPair.Key, validationPair.Value));
            }
        }
        public static ValidationErrors PerformKeyValuePairValidation<T>(this T objectToValidate, (string Key, String Value) validationPair)
        {
            validationPair.Value = validationPair.Value.WithNullConversion();

            var thereAreValidationsButTheObjectIsNull = objectToValidate == null && !string.IsNullOrEmpty(validationPair.Key);
            if (thereAreValidationsButTheObjectIsNull)
                return $"Property was null so we are unable to find '{validationPair.Key}':'{validationPair.Value}'";

            var key = validationPair.Key;
            var expected = validationPair.Value;
            var expectedArray = (expected ?? string.Empty).Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var expectedIsArrayOfValues = (expected ?? string.Empty).Contains(",");

            if (string.IsNullOrWhiteSpace(key)) return new ValidationErrors();

            var objectHierarchy = key.Split(',').ToList();

            var isSingleLevelPropertyString = objectHierarchy.Count == 1;
            if (isSingleLevelPropertyString)
            {
                var theProperty = objectHierarchy[0];
                var thePropertyInfo = objectToValidate.GetType()
                    .GetProperties()
                    .SingleOrDefault(x => x.Name.Equals(theProperty, StringComparison.InvariantCultureIgnoreCase));

                if (thePropertyInfo == null) return $"Property '{theProperty}' is missing in Json {objectToValidate.ToJson()}.";

                var propertyValue = thePropertyInfo.GetValue(objectToValidate);

                return propertyValue.PerformPropertyValidation(expected);
            }
            var objectValidationisNested = objectHierarchy.Count >= 2;
            if (!objectValidationisNested) return ValidationErrors.Empty;

            var childPropertyName = objectHierarchy[0];
            var childProperty = objectToValidate.GetType()
                .GetProperties()
                .Single(propertyInfo => propertyInfo.Name.Equals(childPropertyName, StringComparison.InvariantCultureIgnoreCase))
                .GetValue(objectToValidate);
            if (childProperty == null) return $"Property '{childPropertyName}' is not found in JSON {objectToValidate.ToJson()}.";

            var keyWithoutParentProperty = key.WithoutParentProperty(childPropertyName);
            switch (childProperty)
            {
                case IEnumerable collection:
                    if (expectedIsArrayOfValues)
                        return expectedArray
                            .SelectMany(expectedArrayValue => collection.ValidateExistsInCollection((keyWithoutParentProperty, expectedArrayValue)))
                            .ToValidationErrors();
                    else
                        return collection.ValidateExistsInCollection((keyWithoutParentProperty, expected));

                default:
                    return expectedIsArrayOfValues
                        ? $"Array validation is not available for property '{childPropertyName}' because it is not a collection."
                        : childProperty.PerformKeyValuePairValidation(new KeyValuePair<string, string>(keyWithoutParentProperty, expected));
            }
        }
        public static ValidationErrors PerformPropertyBatchValidation<T>(this T objectToValidate, Dictionary<string, string> directPropertyValidationBatch)
        {
            var allPropertyValidationErrors = directPropertyValidationBatch
                .Select(propertyValidationItem => (
                    Property: propertyValidationItem.Key,
                    ValidationErrors: objectToValidate.PerformKeyValuePairValidation(propertyValidationItem),
                    PropertyValidationItem: propertyValidationItem))
                .ToList();

            var thereAreNoValidationErrors = allPropertyValidationErrors.All(x => !x.ValidationErrors.Any());
            if (thereAreNoValidationErrors)
                return ValidationErrors.Empty;

            var propertiesErrorString = allPropertyValidationErrors.Select(x => $" -'{x.PropertyValidationItem.Key}': '{x.PropertyValidationItem.Value}'");
            return $@"Unable to find '{objectToValidate?.GetType().Name ?? "<Null Object>"}' with
                        Properties
                        {string.Join(Environment.NewLine, propertiesErrorString)}

                        In the following JSON
                        {objectToValidate.ToJson()}";
        }

        public static ValidationErrors PerformPropertyValidation(this object actualSearchedProperty, string expected)
        {
            if (actualSearchedProperty == null)
                return expected != null
                    ? $"Actual: '<NULL>' does not equal Expected: '{expected}'"
                    : ValidationErrors.Empty;

            switch (actualSearchedProperty)
            {
                case decimal actualAsDecimal:
                    return actualAsDecimal.PerformDecimalComparisonTo(expected.ToDecimal());

                case string actualAsString:
                    return actualAsString.Equals(expected)
                        ? ValidationErrors.Empty
                        : $"Actual: '{actualAsString}' does not equal Expected: {expected}";

                default: return $"PerformPropertyValidation - Unhandled type{actualSearchedProperty.GetType()}";
            }

        }
    }
}
