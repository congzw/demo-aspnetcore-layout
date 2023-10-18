using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;

namespace NbApp.Srvs.MdDocs
{
    public class MdDocFile
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
        private readonly IWebHostEnvironment webEnv;

        public MdDocFileService(IWebHostEnvironment webEnv)
        {
            this.webEnv = webEnv;
        }

        public List<MdDocFile> GetMdDocFiles()
        {
            var items = new List<MdDocFile>();

            var mdDir = Path.Combine(webEnv.WebRootPath, "md-files");
            var mdDirInfo = new DirectoryInfo(mdDir);
            if (!mdDirInfo.Exists)
            {
                return items;
            }

            var fileInfos = mdDirInfo.GetFiles("*.md", SearchOption.AllDirectories);
            foreach (var fileInfo in fileInfos)
            {
                items.Add(MdDocFile.Create(mdDir, fileInfo.FullName));
            }

            return items;
        }
    }
}
