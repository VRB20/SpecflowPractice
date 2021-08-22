using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFramework.ObjectGraphBatchValidation.Models
{
    public class Batch : List<BatchItem>
    {
        public Batch() : base(new List<BatchItem>())
        {

        }

        public Batch(IEnumerable<BatchItem> collection) : base(collection)
        {

        }

        public static Batch Default => new Batch();

        public static Batch From(string key, string value) => new Batch(new List<BatchItem>
        {
            new BatchItem
            {
                {key, value }
            }
        });

        public static implicit operator Batch(List<Dictionary<string, string>> dictionary) =>
            new Batch(dictionary.Select(x => new BatchItem(x)));
    }

    public class BatchItem : Dictionary<string, string>
    {
        public BatchItem()
        {

        }

        public BatchItem(Dictionary<string, string> dictionary) : base(dictionary)
        {

        }

        public BatchItem(string key, string value) : base(new Dictionary<string, string>
        {
            { key, value }
        })
        {

        }
    }

    public class ValidationErrors : List<string>
    {
        public ValidationErrors()
        {

        }

        private ValidationErrors(IEnumerable<string> collection) : base(collection)
        {

        }

        public static ValidationErrors Empty => new ValidationErrors();

        public static ValidationErrors From(IEnumerable<string> errors) => new ValidationErrors(errors);

        public static implicit operator ValidationErrors(string error) => new ValidationErrors { error };
    }

    public static class ValidationErrorsExtensions
    {
        public static ValidationErrors ToValidationErrors(this IEnumerable<string> errors) => ValidationErrors.From(errors);
        public static ValidationErrors ToValidationErrors(this IEnumerable<ValidationErrors> results) => ValidationErrors.From(results.SelectMany(x => x));
    }
}
