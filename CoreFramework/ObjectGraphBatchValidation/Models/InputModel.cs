using System.IO.Abstractions;

namespace CoreFramework.ObjectGraphBatchValidation.Models
{
    public class InputModel : InputModel<InputData>
    {
    }

    public class InputModel<T>
    {
        public IFileInfo SourceFile { get; set; }
        public T Data { get; set; }
    }
}
