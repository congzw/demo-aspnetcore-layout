using System.IO;
using System;

namespace NbApp.Srvs.Menus
{
    public static class _ExtForMdFile
    {
        public static PageMenuTree LoadForMdFiles(this PageMenuTreeHelper helper,
            string parentPath,
            string thisName,
            DirectoryInfo dirInfo,
            bool forceLoad = false)
        {
            if (dirInfo is null)
            {
                throw new ArgumentNullException(nameof(dirInfo));
            }
            var cacheKey = $"MdFiles@{dirInfo.FullName}".ToLower();
            var searchFilePattern = "*.md";
            var defaultFileName = "index.md";
            var ignoreFile = (FileInfo fileInfo) => fileInfo.Name.StartsWith("_");
            var ignoreDirectory = (DirectoryInfo dirInfo) => dirInfo.Name.StartsWith("_");
            return helper.Load(parentPath, thisName, dirInfo, cacheKey, forceLoad, searchFilePattern, defaultFileName, ignoreFile, ignoreDirectory);
        }
    }
}