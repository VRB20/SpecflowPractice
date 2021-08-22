using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace CoreFramework.Services
{
    public class InputFileService : IInputFileService
    {
        public List<IFileInfo> GetInputFiles(IDirectoryInfo baseDirectory, SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
            baseDirectory.GetFiles("*.csv", searchOption)
                         .ToList();
    }

    public interface IInputFileService
    {
        List<IFileInfo> GetInputFiles(IDirectoryInfo baseDirectory, SearchOption searchOption = SearchOption.TopDirectoryOnly);
    }
}
