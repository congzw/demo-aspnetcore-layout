using System.IO;
using System;

namespace NbApp.Srvs.Menus
{
    public static class _ExtForStaticFile
    {
        public static PageMenuTree LoadForStaticFiles(this PageMenuTreeHelper helper,
            string parentPath,
            string thisName,
            DirectoryInfo dirInfo,
            bool forceLoad = false)
        {
            if (dirInfo is null)
            {
                throw new ArgumentNullException(nameof(dirInfo));
            }
            var cacheKey = $"{thisName}@{dirInfo.FullName}".ToLower();
            var searchFilePattern = "*.*";
            var defaultFileName = "index";
            var ignoreFile = (FileInfo fileInfo) => fileInfo.Name.StartsWith("_");
            var ignoreDirectory = (DirectoryInfo dirInfo) => dirInfo.Name.StartsWith("_");
            return helper.Load(parentPath, thisName, dirInfo, cacheKey, forceLoad, searchFilePattern, defaultFileName, ignoreFile, ignoreDirectory);
        }
    }

}