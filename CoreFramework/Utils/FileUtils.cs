using System.IO;

namespace CoreFramework.Utils
{
    public class FileUtils
    {
        public bool ChkFileExists(string location)
        {
            if (File.Exists(location))
                return true;
            else
                return false;
        }

    }
}
