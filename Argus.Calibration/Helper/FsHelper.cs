using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic;

namespace Argus.Calibration.Helper
{
    public static class FsHelper
    {
        public static void PurgeDirectory(string path)
        {
            DirectoryInfo di = new(path);
            if (!di.Exists)
            {
                di.Create();
            }

            foreach (var fileInfo in di.GetFiles())
            {
                fileInfo.Delete();
            }
        }

        public static string GetFirstFileByNameFromDirectory(string path, string name)
        {
            DirectoryInfo di = new(path);
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

        public static List<string> GetImageFilesInFolder(string path)
        {
            ISet<string> extensions = new HashSet<string> { ".jpg", ".bmp", ".png" };

            DirectoryInfo imgDir = new DirectoryInfo(path);
            FileInfo[] files = imgDir.GetFiles();

            List<string> imageFiles = new List<string>();
            foreach (FileInfo file in files)
            {
                if (extensions.Contains(file.Extension.ToLower()))
                {
                    imageFiles.Add(file.FullName);
                }
            }

            imageFiles.Sort();

            return imageFiles;
        }
    }
}