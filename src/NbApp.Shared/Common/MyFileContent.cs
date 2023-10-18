using System.Collections.Generic;
using System.IO;

namespace Common
{
    public class MyFileContent
    {
        public string RelativeTo { get; set; }
        public string FullPath { get; set; }

        public string RelativePath => Path.GetRelativePath(RelativeTo, FullPath);
        public string FileName => string.IsNullOrWhiteSpace(FullPath) ? "" : Path.GetFileName(FullPath);
        public string DisplayPath => RelativePath?.Replace("\\", "/");
        private string _content = null;
        public string Content => LoadContentIf();

        public string LoadContentIf()
        {
            if (_content != null)
            {
                return _content;
            }
            if (!File.Exists(FullPath))
            {
                _content = "";
            }
            else
            {
                _content = File.ReadAllText(FullPath);
            }

            return _content;
        }

        public static MyFileContent Create(string relativeTo, string fullPath)
        {
            return new MyFileContent() { RelativeTo = relativeTo, FullPath = fullPath };
        }

        public static List<MyFileContent> Load(string rootPath, string searchPattern, SearchOption searchOption)
        {
            var items = new List<MyFileContent>();

            var rootDirInfo = new DirectoryInfo(Path.GetFullPath(rootPath));
            if (!rootDirInfo.Exists)
            {
                return items;
            }

            var fileInfos = rootDirInfo.GetFiles(searchPattern, searchOption);
            foreach (var fileInfo in fileInfos)
            {
                items.Add(Create(rootDirInfo.FullName, fileInfo.FullName));
            }

            return items;
        }
    }
}