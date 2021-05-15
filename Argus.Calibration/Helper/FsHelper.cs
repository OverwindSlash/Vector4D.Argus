using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            foreach (var directory in di.GetDirectories())
            {
                PurgeDirectory(directory.FullName);
            }

            foreach (var fileInfo in di.GetFiles())
            {
                fileInfo.Delete();
            }
        }

        public static void EnsureDirectoryExist(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static string GetFirstFileByNameFromDirectory(string path, string name)
        {
            DirectoryInfo di = new(path);
            if (!di.Exists)
            {
                return string.Empty;
            }

            List<FileInfo> fis = di.GetFiles().ToList();
            fis.Sort((l, r) => l.Name.CompareTo(r.Name));

            var result = fis.FirstOrDefault(fi => fi.Name.ToLower().Contains(name));

            return result.FullName;
        }

        public static string GetLastFileByNameFromDirectory(string path, string name)
        {
            DirectoryInfo di = new(path);
            if (!di.Exists)
            {
                return string.Empty;
            }

            List<FileInfo> fis = di.GetFiles().ToList();
            fis.Sort((l, r) => -l.Name.CompareTo(r.Name));

            var result = fis.FirstOrDefault(fi => fi.Name.ToLower().Contains(name));

            return result.FullName;
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