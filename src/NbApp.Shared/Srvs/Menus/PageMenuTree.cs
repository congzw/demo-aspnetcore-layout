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
        public object Tag { get; set; }
        public bool LinkDisabled { get; set; }

        public static T Create<T>(string id, string parentId, string title, string icon, string herf) where T : PageMenuTree, new()
        {
            return new T()
            {
                Id = id,
                ParentId = parentId,
                Title = title,
                Icon = icon,
                Herf = herf
            };
        }
    }

    public class PageMenuTreeHelper
    {
        public static PageMenuTreeHelper Instance = new PageMenuTreeHelper();
        internal static IDictionary<string, PageMenuTree> Cache { get; set; } = new Dictionary<string, PageMenuTree>(StringComparer.OrdinalIgnoreCase);

        private PathHelper _pathHelper = PathHelper.Instance;

        public PageMenuTree Load(string pageLocation, bool force = false)
        {
            var hasValue = Cache.TryGetValue(pageLocation, out var _menu);
            if (force || !hasValue)
            {
                var dirInfo = new DirectoryInfo(pageLocation);
                if (dirInfo.Exists)
                {
                    _menu = CreateMenuTree(dirInfo, "");
                    Cache[pageLocation] = _menu;
                }
            }
            return _menu;
        }

        private PageMenuTree CreateMenuTree(DirectoryInfo dirInfo, string parentPath)
        {
            //foo/bar/blah.cshtml
            var thisDirPath = (parentPath + "/" + dirInfo.Name).ToLower();
            //var thisDirTree = MenuItem.Create<PageMenuTree>(thisDirPath, parentPath, dirInfo.Name, "", "javascript:void(0)");

            var thisDirTree = PageMenuTree.Create<PageMenuTree>(thisDirPath, parentPath, dirInfo.Name, "", thisDirPath);
            thisDirTree.Tag = dirInfo.FullName;

            var theIndexFileExist = dirInfo.GetFiles("*.cshtml", SearchOption.TopDirectoryOnly).Any(x => x.Name.Equals("index.cshtml", StringComparison.OrdinalIgnoreCase));
            thisDirTree.LinkDisabled = !theIndexFileExist;

            var fileInfos = dirInfo.GetFiles("*.cshtml", SearchOption.TopDirectoryOnly);
            foreach (var fileInfo in fileInfos)
            {
                if (fileInfo.Name.StartsWith("_"))
                {
                    continue;
                }
                var thisFilePath = (thisDirPath + "/" + fileInfo.Name).ToLower().Replace(".cshtml", "");
                var fileTree = PageMenuTree.Create<PageMenuTree>(thisFilePath, parentPath, Path.GetFileNameWithoutExtension(fileInfo.Name), "", thisFilePath);
                fileTree.Tag = fileInfo.FullName;
                thisDirTree.Children.Add(fileTree);
            }

            var subDirInfos = dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (var subDirInfo in subDirInfos)
            {
                if (subDirInfo.Name.Equals("Shared") || subDirInfo.Name.Equals("Components"))
                {
                    continue;
                }
                var dirTree = CreateMenuTree(subDirInfo, thisDirPath);
                thisDirTree.Children.Add(dirTree);
            }
            return thisDirTree;
        }
    }
}