using System;
using System.IO;
using Microsoft.VisualBasic;

namespace Argus.Calibration.Helper
{
    public static class FsHelper
    {
        public static void PurgeDirectory(string dir)
        {
            DirectoryInfo di = new(dir);
            if (!di.Exists)
            {
                di.Create();
            }

            foreach (var fileInfo in di.GetFiles())
            {
                fileInfo.Delete();
            }
        }

        public static string GetFirstFileByNameFromDirectory(string dir, string name)
        {
            DirectoryInfo di = new(dir);
            if (!di.Exists)
            {
                return string.Empty;
            }
            
            foreach (var fileInfo in di.GetFiles())
            {
                if (fileInfo.Name.ToLower().Contains(name))
                {
                    return fileInfo.FullName;
                }
            }

            return string.Empty;
        }
    }
}