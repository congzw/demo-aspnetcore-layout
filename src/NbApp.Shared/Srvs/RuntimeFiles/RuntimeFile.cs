using Common;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NbApp.Srvs.RuntimeFiles
{
    public class RuntimeFile
    {
        public string FileName { get; set; }
        public string Position { get; set; }

        public string FullPath { get; set; }
        public string Link { get; set; }

        private string content;
        public string Content
        {
            get
            {
                if (content == null)
                {
                    content = LoadFileContentIf(FullPath);
                }
                return content;
            }
            set
            {
                content = value;
            }
        }

        public async Task SaveContentIf(string newContent, Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(FullPath))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(newContent))
            {
                newContent = "";
            }

            if (File.Exists(FullPath))
            {
                PathHelper.Instance.MakeSureParentDirExist(FullPath);
                await File.WriteAllTextAsync(FullPath, newContent, encoding ?? Encoding.UTF8);
                Content = newContent;
            }
            else
            {
                var md5 = HashHelper.Instance.GetMd5();
                var oldHash = md5.ComputeHashString(Content);
                var newHash = md5.ComputeHashString(newContent);
                if (oldHash != newHash)
                {
                    PathHelper.Instance.MakeSureParentDirExist(FullPath);
                    await File.WriteAllTextAsync(FullPath, newContent, encoding ?? Encoding.UTF8);
                    Content = newContent;
                }
            }
        }

        private string LoadFileContentIf(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return "";
            }
            if (!File.Exists(fullPath))
            {
                return "";
            }
            return File.ReadAllText(fullPath);
        }
    }

    public class RuntimeFileHelper
    {
        public static RuntimeFileHelper Instance = new RuntimeFileHelper();

        public string SaveRoot => MyAppContext.Instance.GetRuntimeStaticDirectory();
        public string LinkBase => MyAppContext.Instance.LinkBase;

        public RuntimeFile Create(string position, string fileName)
        {
            var fixPostion = FixPosition(position);
            var item = new RuntimeFile();
            item.FileName = fileName;
            item.Position = fixPostion;
            item.Link = GuessLink(fixPostion, fileName, false);
            item.FullPath = GuessFullPath(fixPostion, fileName, false);
            return item;
        }

        public string FixPosition(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
            {
                return "";
            }
            return position.Trim().Replace('\\','/').TrimStart('/').TrimEnd('/');
        }

        public string GuessLink(string position, string fileName, bool autoFixPostion = true)
        {
            var fixPosition = position;
            if (autoFixPostion)
            {
                fixPosition = FixPosition(position);
            }

            return $"{LinkBase}/{fixPosition}/{fileName}".ToLower();
        }

        public string GuessFullPath(string position, string fileName, bool autoFixPostion = true)
        {
            var fixPosition = position;
            if (autoFixPostion)
            {
                fixPosition = FixPosition(position);
            }

            var subs = fixPosition.Split('/');
            var fullPath = SaveRoot;
            foreach (var sub in subs)
            {
                fullPath = Path.Combine(fullPath, sub.ToLower());
            }
            return Path.Combine(fullPath, fileName);
        }
    }
}