using Common;
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

        internal FileSystemInfo FileSystemInfo { get; set; }
        public FileSystemInfo GetFileSystemInfo() => FileSystemInfo;
        public string GetFullPath() => FileSystemInfo?.FullName;
        public bool IsDirectory => FileSystemInfo is DirectoryInfo;

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

            var fullPath = this.GetFullPath();
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

        public static T Create<T>(string id, string parentId, string title, string icon, string herf, FileSystemInfo info) where T : PageMenuTree, new()
        {
            return new T()
            {
                Id = id,
                ParentId = parentId,
                Title = title,
                Icon = icon,
                Herf = herf,
                FileSystemInfo = info
            };
        }

        public static string CombineHerf(string basePath, string lastPath, string fixStartWith = "/", string fixEndWith = "")
        {
            var nakeBasePath = "";
            if (!string.IsNullOrWhiteSpace(basePath))
            {
                nakeBasePath = basePath.Trim().TrimEnd('/').TrimEnd('/');
            }

            var nakeAppend = "";
            if (!string.IsNullOrWhiteSpace(lastPath))
            {
                nakeAppend = lastPath.Trim().TrimEnd('/').TrimEnd('/'); 
            }

            var combine = fixStartWith + nakeBasePath + '/' + nakeAppend + fixEndWith;
            return combine.Replace("//", "/").Replace("//", "/");
        }
    }
    public class PageMenuTreeHelper
    {
        public static PageMenuTreeHelper Instance = new PageMenuTreeHelper();
        internal IDictionary<string, PageMenuTree> Cache { get; set; } = new Dictionary<string, PageMenuTree>(StringComparer.OrdinalIgnoreCase);
        public void ClearCache()
        {
            Cache.Clear();
        }

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
                else
                {
                    throw new ArgumentException("目录不存在", nameof(dirInfo));
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
            var thisDirHerf = PageMenuTree.CombineHerf(parentPath, thisName).ToLower();
            var thisDirTree = PageMenuTree.Create<PageMenuTree>(thisDirHerf, parentPath, dirInfo.Name, "", thisDirHerf, dirInfo);

            //defaultFileName search order: ["index.foo", "index"]
            var subFileInfos = dirInfo.GetFiles(searchFilePattern, SearchOption.TopDirectoryOnly);
            var theIndexFileExist = subFileInfos.Any(x => x.Name.Equals(defaultFileName, StringComparison.OrdinalIgnoreCase));
            if (!theIndexFileExist)
            {                
                theIndexFileExist = subFileInfos.Any(x => Path.GetFileNameWithoutExtension(x.Name).Equals(defaultFileName, StringComparison.OrdinalIgnoreCase));
            }

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
                //var thisFileHerf = (thisDirHerf + "/" + nameWithoutExtension).ToLower();
                var thisFileHerf = PageMenuTree.CombineHerf(thisDirHerf, nameWithoutExtension).ToLower();
                var fileTree = PageMenuTree.Create<PageMenuTree>(thisFileHerf, parentPath, nameWithoutExtension, "", thisFileHerf, fileInfo);
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
}