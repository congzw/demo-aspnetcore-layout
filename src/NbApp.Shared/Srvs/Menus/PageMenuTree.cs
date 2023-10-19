using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NbApp.Srvs.Menus
{
    public class PageMenuTree
    {
        public List<PageMenuTree> Children { get; set; } = new List<PageMenuTree>();
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Herf { get; set; }
        public object Tag { get; set; }
        public bool IsDirectory { get; set; }
        public bool LinkDisabled { get; set; }
        public int TotalFilesCount() => GetAllDescendants().Count(x => !x.IsDirectory);
        public IEnumerable<PageMenuTree> GetAllDescendants()
        {
            //祖先: Ancestor
            //后裔: Descendant
            foreach (var item in Children)
            {
                yield return item;
                foreach (var c in item.GetAllDescendants())
                {
                    yield return c;
                }
            }
        }

        public string GuessParentHerf()
        {
            if (string.IsNullOrWhiteSpace(Herf))
            {
                return "";
            }
            var parent = Herf.Substring(0, Herf.LastIndexOf('/'));
            return parent;
        }

        private string _content = null;
        public string LoadFileContent(bool forceReload = false)
        {
            if (IsDirectory)
            {
                return "";
            }

            if (_content != null && !forceReload)
            {
                return _content;
            }

            var fullPath = this.Tag?.ToString();
            if (!File.Exists(fullPath))
            {
                _content = "";
            }
            else
            {
                _content = File.ReadAllText(fullPath);
            }

            return _content;
        }

        public static T Create<T>(string id, string parentId, string title, string icon, string herf, bool isDirectory) where T : PageMenuTree, new()
        {
            return new T()
            {
                Id = id,
                ParentId = parentId,
                Title = title,
                Icon = icon,
                Herf = herf,
                IsDirectory = isDirectory
            };
        }
    }

    public class PageMenuTreeHelper
    {
        public static PageMenuTreeHelper Instance = new PageMenuTreeHelper();
        internal IDictionary<string, PageMenuTree> Cache { get; set; } = new Dictionary<string, PageMenuTree>(StringComparer.OrdinalIgnoreCase);

        public PageMenuTree Load(
            string parentPath,
            string thisName,
            DirectoryInfo dirInfo,
            string cacheKey,
            bool forceReload,
            string searchFilePattern,
            string defaultFileName,
            Func<FileInfo, bool> ignoreFile,
            Func<DirectoryInfo, bool> ignoreDirectory)
        {
            var hasValue = Cache.TryGetValue(cacheKey, out var _menu);
            if (forceReload || !hasValue)
            {
                if (dirInfo.Exists)
                {
                    _menu = CreateMenuTree(parentPath, thisName, dirInfo, searchFilePattern, defaultFileName, ignoreFile, ignoreDirectory);
                    Cache[cacheKey] = _menu;
                }
            }
            return _menu;
        }

        private PageMenuTree CreateMenuTree(
            string parentPath,
            string thisName,
            DirectoryInfo dirInfo,
            string searchFilePattern, 
            string defaultFileName,
            Func<FileInfo, bool> ignoreFile,
            Func<DirectoryInfo, bool> ignoreDirectory)
        {
            //foo/bar/blah.cshtml
            //foo/bar/blah.md
            var thisDirHerf = (parentPath.TrimEnd('/') + "/" + thisName).ToLower();
            var thisDirTree = PageMenuTree.Create<PageMenuTree>(thisDirHerf, parentPath, dirInfo.Name, "", thisDirHerf, true);
            thisDirTree.Tag = dirInfo.FullName;

            var theIndexFileExist = dirInfo.GetFiles(searchFilePattern, SearchOption.TopDirectoryOnly)
                .Any(x => x.Name.Equals(defaultFileName, StringComparison.OrdinalIgnoreCase));
            thisDirTree.LinkDisabled = !theIndexFileExist;

            var fileInfos = dirInfo.GetFiles(searchFilePattern, SearchOption.TopDirectoryOnly);
            foreach (var fileInfo in fileInfos)
            {
                if (ignoreFile != null)
                {
                    if (ignoreFile(fileInfo))
                    {
                        continue;
                    }
                }

                var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
                var thisFileHerf = (thisDirHerf + "/" + nameWithoutExtension).ToLower();
                var fileTree = PageMenuTree.Create<PageMenuTree>(thisFileHerf, parentPath, nameWithoutExtension, "", thisFileHerf, false);
                fileTree.Tag = fileInfo.FullName;
                thisDirTree.Children.Add(fileTree);
            }

            var subDirInfos = dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (var subDirInfo in subDirInfos)
            {
                if (ignoreDirectory != null)
                {
                    if (ignoreDirectory(subDirInfo))
                    {
                        continue;
                    }
                }
                var dirTree = CreateMenuTree(thisDirHerf, subDirInfo.Name, subDirInfo, searchFilePattern, defaultFileName, ignoreFile, ignoreDirectory);
                thisDirTree.Children.Add(dirTree);
            }
            return thisDirTree;
        }
    }

    public static class PageMenuTreeHelperExtensions
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