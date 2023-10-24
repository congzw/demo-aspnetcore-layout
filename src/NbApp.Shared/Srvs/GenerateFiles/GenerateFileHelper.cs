using System;
using System.IO;
using System.Linq;

namespace NbApp.Srvs.GenerateFiles
{
    public class GenerateFile
    {
        public string FileName { get; set; }
        public string Href { get; set; }
        public string FullPath { get; set; }

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

        public string GuessModelFileFullPath()
        {
            //foo.service => _foo.service.json
            if (string.IsNullOrWhiteSpace(FullPath))
            {
                return "";
            }
            return FullPath.Replace(FileName, $"_{FileName}.json");
        }
        private string modelJson;
        public string ModelJson
        {
            get
            {
                if (modelJson == null)
                {
                    modelJson = LoadFileContentIf(FullPath);
                }
                return modelJson;
            }
            set
            {
                modelJson = value;
            }
        }
    }

    public class GenerateFileHelper
    {
        public static GenerateFileHelper Instance = new GenerateFileHelper();

        public string SaveRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "generate-files");
        public string HrefBase = "/app/script-doc";
        public string RazorViewPathTrimStart = "/Pages/App/Generate/";

        public string GuessHref(string razorViewPath, string fileName)
        {
            var subValue = razorViewPath.Substring(0, razorViewPath.LastIndexOf('/')).Replace(RazorViewPathTrimStart, "");
            return $"{HrefBase}/{subValue}/{fileName}".ToLower();
        }

        public string GuessFullPath(string razorViewPath, string fileName)
        {
            var subValue = razorViewPath.Substring(0, razorViewPath.LastIndexOf('/')).Replace(RazorViewPathTrimStart, "");
            var subs = subValue.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var fullPath = SaveRoot;
            foreach (var sub in subs)
            {
                fullPath = Path.Combine(fullPath, sub);
            }
            return Path.Combine(fullPath, fileName);
        }

        public GenerateFile Create(string razorViewPath, string fileName)
        {
            var item = new GenerateFile();
            item.FileName = fileName;
            item.Href = GuessHref(razorViewPath, fileName);
            item.FullPath = GuessFullPath(razorViewPath, fileName);
            return item;
        }
    }
}