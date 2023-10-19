using System;
using System.Collections.Generic;
using System.IO;

namespace NbApp.Srvs.MdDocs
{
    public class MdDocFile
    {
        public string Id { get; set; }
        public string RelativeTo { get; set; }
        public string FullPath { get; set; }

        public string RelativePath => Path.GetRelativePath(RelativeTo, FullPath);
        public string FileName => string.IsNullOrWhiteSpace(FullPath) ? "" : Path.GetFileName(FullPath);
        public string DisplayPath => RelativePath?.Replace("\\", "/");

        private string _content = null;
        public string LoadContent(bool forceReload = false)
        {
            if (_content != null && !forceReload)
            {
                return _content;
            }

            var fullPath = Path.Combine(RelativeTo, RelativePath);
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

        public static MdDocFile Create(string relativeTo, string fullPath)
        {
            return new MdDocFile() { RelativeTo = relativeTo, FullPath = fullPath };
        }
    }

    public class MdDocFileService
    {
        public List<MdDocFile> GetMdDocFiles(string rootDir)
        {
            var items = new List<MdDocFile>();

            var mdDirInfo = new DirectoryInfo(rootDir);
            if (!mdDirInfo.Exists)
            {
                return items;
            }

            var fileInfos = mdDirInfo.GetFiles("*.md", SearchOption.AllDirectories);
            foreach (var fileInfo in fileInfos)
            {
                items.Add(MdDocFile.Create(rootDir, fileInfo.FullName));
            }

            return items;
        }
    }

    public class MdDocFileMeta
    {
        public string Id { get; set; }
        public string Author { get; set; } = "";
        public DateTime CreateAt { get; set; } = new DateTime(2000, 1, 1);
        public DateTime UpdateAt { get; set; } = new DateTime(2000, 1, 1);
        public string Summary { get; set; } = "";
        public string Tags { get; set; } = "";
    }
}
