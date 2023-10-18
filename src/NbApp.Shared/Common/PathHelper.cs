using System.Collections.Generic;
using System.IO;
using System;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class PathHelper
    {
        public static PathHelper Instance = new PathHelper();

        public FileRelativePath GuessForFile(string fileFullPath, string relativeTo, bool appendRelative = false)
        {
            if (string.IsNullOrWhiteSpace(fileFullPath))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(relativeTo))
            {
                return null;
            }

            var lastIndex = fileFullPath.LastIndexOf(relativeTo, StringComparison.OrdinalIgnoreCase);
            if (lastIndex < 0)
            {
                return null;
            }

            var thePath = fileFullPath.Substring(lastIndex, fileFullPath.Length - lastIndex).Replace("\\", "/");
            if (appendRelative)
            {
                return new FileRelativePath { RelativePath = thePath, FileFullPath = fileFullPath };
            }
            return new FileRelativePath { RelativePath = thePath.Replace(relativeTo + "/", "", StringComparison.OrdinalIgnoreCase), FileFullPath = fileFullPath };
        }
        public List<FileRelativePath> CreateForDir(string dirFullName, string pattren = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            var theResult = new List<FileRelativePath>();
            if (!Directory.Exists(dirFullName))
            {
                return theResult;
            }

            var files = Directory.GetFiles(dirFullName, pattren, searchOption);
            foreach (var fileFullPath in files)
            {
                theResult.Add(CreateForFile(dirFullName, fileFullPath));
            }
            return theResult;
        }
        public FileRelativePath CreateForFile(string dirFullPath, string fileFullPath)
        {
            var thePath = Path.GetRelativePath(dirFullPath, fileFullPath).Replace("\\", "/");
            return new FileRelativePath { RelativePath = thePath, FileFullPath = fileFullPath };
        }

        public void MakeSureParentDirExist(string fullPath)
        {
            var theDir = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrWhiteSpace(theDir))
            {
                return;
            }
            if (!Directory.Exists(theDir))
            {
                Directory.CreateDirectory(theDir);
            }
        }
    }

    public class FileRelativePath
    {
        public string FileFullPath { get; set; }
        public string RelativePath { get; set; }
    }
}