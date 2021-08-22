using CoreFramework.ObjectGraphBatchValidation.Models;
using System.Collections.Generic;

namespace CoreFramework.ObjectGraphBatchValidation
{
    public static class ValidationBatchExtensions
    {
        public static BatchItem ToValidationBatchItem(this Dictionary<string, string> dictionary) => new BatchItem(dictionary);
    }
}
