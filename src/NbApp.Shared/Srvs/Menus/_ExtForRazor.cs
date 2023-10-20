using System.IO;
using System;

namespace NbApp.Srvs.Menus
{
    public static class _ExtForRazor
    {
        public static PageMenuTree LoadForRazorPages(this PageMenuTreeHelper helper,
            string parentPath,
            string thisName,
            DirectoryInfo dirInfo,
            bool forceLoad = false)
        {
            if (dirInfo is null)
            {
                throw new ArgumentNullException(nameof(dirInfo));
            }
            var cacheKey = $"RazorPages@{dirInfo.FullName}".ToLower();
            var searchFilePattern = "*.cshtml";
            var defaultFileName = "Index.cshtml";
            var ignoreFile = (FileInfo fileInfo) => fileInfo.Name.StartsWith("_");
            var ignoreDirectory = (DirectoryInfo dirInfo) => dirInfo.Name.Equals("Shared") || dirInfo.Name.Equals("Components") || dirInfo.Name.StartsWith("_");
            return helper.Load(parentPath, thisName, dirInfo, cacheKey, forceLoad, searchFilePattern, defaultFileName, ignoreFile, ignoreDirectory);
        }
    }
}