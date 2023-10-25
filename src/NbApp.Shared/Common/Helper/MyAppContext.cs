using System;
using System.IO;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class MyAppContext
    {
        public static MyAppContext Instance = new MyAppContext();

        public string Root = "wwwroot/runtime-static";
        public string LinkBase = "/runtime-static";

        public FileSystemWatcher Watcher { get; set; }
        public Action<FileSystemEventArgs> WatcherChanged { get; set; }
    }

    public static class MyAppContextExtentions
    {
        public static MyAppContext SetupFileWatch(this MyAppContext ctx)
        {
            var rootPath = ctx.GetRuntimeStaticDirectory();
            var watcher = new FileSystemWatcher(rootPath, "*.*");

            //todo: fix bugs!
            //watcher.NotifyFilter = NotifyFilters.LastWrite;
            ////| NotifyFilters.Security
            ////| NotifyFilters.Size;

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 //| NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite;
                                //| NotifyFilters.Security
                                //| NotifyFilters.Size;

            watcher.Changed += (object sender, FileSystemEventArgs e) => ctx.WatcherChanged?.Invoke(e);
            watcher.Created += (object sender, FileSystemEventArgs e) => ctx.WatcherChanged?.Invoke(e);
            watcher.Deleted += (object sender, FileSystemEventArgs e) => ctx.WatcherChanged?.Invoke(e);
            watcher.Renamed += (object sender, RenamedEventArgs e) => ctx.WatcherChanged?.Invoke(e);

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents= true;
            ctx.Watcher = watcher;

            return ctx;
        }

        public static string GetRuntimeStaticDirectory(this MyAppContext ctx, params string[] subs)
        {
            var basePath = AppContext.BaseDirectory;
            var positionSubs = ctx.Root.Trim().TrimStart('/').TrimEnd('/').Split('/').ToArray();
            foreach (var sub in positionSubs)
            {
                basePath = Path.Combine(basePath, sub);
            }

            foreach (var sub in subs)
            {
                if (string.IsNullOrWhiteSpace(sub))
                {
                    continue;
                }
                basePath = Path.Combine(basePath, sub.Trim());
            }
            return basePath;
        }
    }
}