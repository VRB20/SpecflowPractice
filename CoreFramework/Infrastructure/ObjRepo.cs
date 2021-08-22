using CoreFramework.Extensions;
using CoreFramework.Services;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;


namespace CoreFramework.Infrastructure
{
    public class ObjRepo
    {
        public static readonly IDirectoryInfo objRepoFolder = typeof(ObjRepo).Assembly.GetFolder(@"ObjectRepository");

        public static Dictionary<string, Dictionary<string, string>> LoadObjectRepository()
        {
            Console.WriteLine("Object Repo Path : " + objRepoFolder.FullName.ToString());
            return ObjectRepoService.LoadObjectRepo(objRepoFolder);
        }
    }
}
