using CoreFramework.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace CoreFramework.Services
{
    public class ObjectRepoService
    {
        public static Dictionary<string, Dictionary<string, string>> objRepoBatch = new Dictionary<string, Dictionary<string, string>>();
        //public static Dictionary<string, Dictionary<string, string>> objRepoBatch;
        public static Dictionary<string, Dictionary<string, string>> LoadObjectRepo(IDirectoryInfo objRepoFolder, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var objRepoInputFiles = new InputFileService().GetInputFiles(objRepoFolder, searchOption);
            objRepoBatch = objRepoInputFiles.SelectMany(input => input.ObjRepoBatch()).ToDictionary(o => o.Key, o => o.Value);
            return objRepoBatch;
        }

        public static (string locator, string path) ObjRepoBatchConstruction(String identifier)
        {
            var objRepoBatchItem = objRepoBatch.Where(item => item.Value["Identifier"].Equals(identifier, StringComparison.OrdinalIgnoreCase))
                        .ToDictionary(x => x.Key, x => x.Value).First();
            var locator = objRepoBatchItem.Value
                .Where(x => x.Key.Equals("Locator", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x.Key, x => x.Value).First().Value;
            var path = objRepoBatchItem.Value
                .Where(x => x.Key.Equals("Path", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x.Key, x => x.Value).First().Value;

            return (locator, path);
        }
    }
}
