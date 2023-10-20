using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class MyCacheLog
    {
        public static MyCacheLog Instance = new MyCacheLog();

        public void LogInfo(string msg, string category = "")
        {
            Logs.Add($"[{category}]{DateTime.Now:HH:mm:ss:fff} => {msg}");
        }

        public List<string> Logs { get; set; } = new List<string>();
    }
}
