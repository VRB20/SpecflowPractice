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
    public static class ObjRepoInputFileProcessingExtension
    {
        public static Dictionary<string, Dictionary<string, string>> ObjRepoBatch(this IFileInfo inputFile)
        {
            var records = new Batch();
            var records1 = new Dictionary<string, Dictionary<string, string>>();

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

                    records1.Add(values[headers.IndexOf("Identifier")], headers
                        .Select((header, index) => new KeyValuePair<string, string>(header, values[index]))
                        .ToList()
                        .ToDictionary(x => x.Key, x => x.Value)
                        );
                }

                //records = records.RemoveEmptyRows();
                return records1;
                //return records;
            }
        }
    }
}
