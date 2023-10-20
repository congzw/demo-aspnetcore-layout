using System;
using System.IO;

// ReSharper disable once CheckNamespace
namespace Common
{
    /// <summary>
    /// 思路：
    /// 1. 项目文件中l定义为$(BuildBranch)， 全量JSON定义在build_info.txt中
    /// 2. 构建发布时，支持替换: dotnet publish 【此处省略若干参数...】 -p:IsPublish=true -p:BuildCommit= -p:BuildTag= -p:BuildBranch= -p:BuildMemo=
    /// </summary>
    public class BuildInfoFile
    {
        public bool IsPublish { get; set; } = false;
        public DateTime BuildAt { get; set; } = new DateTime(2000, 1, 1);
        public DateTime CommitAt { get; set; } = new DateTime(2000, 1, 1);
        public string BuildCommit { get; set; } = "";
        public string BuildTag { get; set; } = "";
        public string BuildBranch { get; set; } = "";
        public string BuildMemo { get; set; } = "";

        public static string build_info_file_name = "build_info.txt";
        private static BuildInfoFile _cache = null;
        public static BuildInfoFile Load(bool forceRead = false, string saveDir = null)
        {
            if (_cache != null && !forceRead)
            {
                return _cache;
            }
            if (string.IsNullOrWhiteSpace(saveDir))
            {
                saveDir = AppDomain.CurrentDomain.BaseDirectory;
            }
            var saveFilePath = Path.Combine(saveDir, build_info_file_name);
            var theData = JsonFileData.Create(saveFilePath);
            return theData.LoadFromFile<BuildInfoFile>(null);
        }

        public static void Save(BuildInfoFile model, string saveDir = null)
        {
            if (string.IsNullOrWhiteSpace(saveDir))
            {
                saveDir = AppDomain.CurrentDomain.BaseDirectory;
            }

            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            var saveFilePath = Path.Combine(saveDir, build_info_file_name);
            var theData = JsonFileData.Create(saveFilePath);
            theData.SaveToFile(model);
        }
    }
}