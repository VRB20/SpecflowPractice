using CoreFramework.Extensions;
using CoreFramework.ObjectGraphBatchValidation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;

namespace CoreFramework.Infrastructure.Extensions
{
    public static class TestDataExtension
    {
        //public static IDirectoryInfo GetTestDataFolder = typeof(TestDataExtension).Assembly.GetFolder("TestData");
        public static Dictionary<string, BatchItem> TestDataBatch;
        public static Dictionary<string, BatchItem> ForexDataBatch;

        public static IDirectoryInfo GetTestDataFolderDetails(this Assembly executingAssembly)
        {
            return executingAssembly.GetFolder("TestData");
        }

        public static Dictionary<string, BatchItem> TestDataBatchConstruction(this Assembly executingAssembly, String identifierKey = "TestcaseName")
        {
            var GetTestDataFolder = executingAssembly.GetFolder("TestData");
            var inputFiles = GetTestDataFolder.GetFiles("*.csv", SearchOption.AllDirectories);
            TestDataBatch = inputFiles.ToTestCase(identifierKey);
            return TestDataBatch;
        }

        //public static Dictionary<string, BatchItem> ForexBatchConstruction(string identifierKey = "Target_CURRENCY")
        //{
        //    var inputFiles = GetTestDataFolder.GetFiles("FXRates*.csv", SearchOption.AllDirectories);
        //    ForexDataBatch = inputFiles.ToTestCase(identifierKey);
        //    return ForexDataBatch;
        //}

        public static Dictionary<String, BatchItem> ToTestCase(this IFileInfo[] inputFiles, string identifierKey) =>
            inputFiles.SelectMany(inputFile => inputFile.ToTestCase(identifierKey))
            .ToDictionary(x => x.Key, x => x.Value);

        public static Dictionary<string, BatchItem> ToTestCase(this IFileInfo inputFile, string identifierKey)
        {
            var inputTestData = inputFile.ToInputModelOfValidationBatchItems(false);
            var testCaseData = inputTestData.Data.Select(x => x[identifierKey]).Distinct();
            var testCaseBatches = testCaseData.ToDictionary(x => x,
                testCaseName => inputTestData.Data.Where(batchItem => batchItem[identifierKey].Equals(testCaseName)).ToBatch());
            var testCaseBatch = testCaseBatches.ToDictionary(x => x.Key,
                x =>
                {
                    var batchValue = x.Value.SelectMany(batchItem => batchItem).ToDictionary(y => y.Key, y => y.Value).ToBatchItem();
                    return batchValue;
                });

            return testCaseBatch;
        }
        public static string GetTestData(this string identifier, string columnName)
        {
            var testData = TestDataBatch.Where(x => x.Key.Equals(identifier, StringComparison.OrdinalIgnoreCase))
                            .Select(batchItem => batchItem.Value.Where(item => item.Key.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                            .Select(id => id.Value).ToList().First()).First();
            testData = testData.Contains("\"") ? testData.Replace("\"", "") : testData;
            return testData;
        }
    }
}
