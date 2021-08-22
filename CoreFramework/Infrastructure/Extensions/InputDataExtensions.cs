using CoreFramework.Extensions;
using CoreFramework.ObjectGraphBatchValidation;
using CoreFramework.ObjectGraphBatchValidation.Models;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace CoreFramework.Infrastructure.Extensions
{
    public static class InputDataExtensions
    {
        public static InputModel<Batch> ToInputModelOfValidationBatchItems(this IFileInfo inputFile, bool requiredTestCaseKey = true)
        {
            $"Extracting data from '{inputFile.FullName}'".Log();

            var records = new Batch();
            using (var reader = new StreamReader(inputFile.FullName))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.MissingFieldFound = null;
                csv.Configuration.HeaderValidated = null;

                var isHeader = true;
                var headers = new List<string>();
                while (csv.Read())
                {
                    if (isHeader)
                    {
                        csv.ReadHeader();
                        headers = csv.Context.HeaderRecord.ToList();
                        isHeader = false;
                        continue;
                    }

                    var values = csv.Context.Record;
                    records.Add(headers
                        .Select((header, index) => new KeyValuePair<string, string>(header, values[index]))
                        .ToList()
                        .ToDictionary(x => x.Key, x => x.Value)
                        .RemoveEmptyColumns()
                        .ToValidationBatchItem());
                }
            }

            if (requiredTestCaseKey) records = records.Where(x => x.Keys.Any(KeyNotFoundException => x.ContainsKey("TestCase"))).ToList().ToBatch();

            records = records.RemoveEmptyRows();

            return new InputModel<Batch>
            {
                SourceFile = inputFile,
                Data = records
            };
        }
    }
}
