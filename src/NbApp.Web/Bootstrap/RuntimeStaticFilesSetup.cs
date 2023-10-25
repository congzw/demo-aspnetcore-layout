
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;
using Common;
using NbApp.Srvs.Menus;

namespace NbApp.Web.Bootstrap
{
    public static class RuntimeStaticFilesSetup
    {
        public static void UseRuntimeStatics(this IApplicationBuilder app, string customLocation = null)
        {
            //开发时，动态补充StaticFiles
            MyAppContext.Instance.WatcherChanged = (e) =>
            {
                System.Console.WriteLine($" >>>> {e.ChangeType} {e.Name} => {Path.GetFileName(e.FullPath)}");
                PageMenuTreeHelper.Instance.ClearCache();
            };
            
            var myAppCtx = MyAppContext.Instance.SetupFileWatch();
            var theRoot = myAppCtx.GetRuntimeStaticDirectory();
            var linkBase = myAppCtx.LinkBase;
            if (!string.IsNullOrWhiteSpace(customLocation))
            {
                theRoot = customLocation;
            }
            if (!Directory.Exists(theRoot))
            {
                Directory.CreateDirectory(theRoot);
            }

            var opt = new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(theRoot),
                RequestPath = linkBase,
                EnableDirectoryBrowsing = true
            };
            opt.StaticFileOptions.ContentTypeProvider = StaticFileServerHelper.Instance.CreateFileExtensionContentTypeProvider();
            app.UseFileServer(opt);
        }
    }

    public class StaticFileServerHelper
    {
        public static StaticFileServerHelper Instance = new StaticFileServerHelper();

        public FileExtensionContentTypeProvider CreateFileExtensionContentTypeProvider()
        {
            var typeProvider = new FileExtensionContentTypeProvider();
            //foreach (var item in typeProvider.Mappings)
            //{
            //    Console.WriteLine(item.Key + ":" + item.Value);
            //}
            //乱码问题
            typeProvider.Mappings[".md"] = "text/markdown; charset=utf-8";
            typeProvider.Mappings[".service"] = "text/plain; charset=utf-8";
            typeProvider.Mappings[".bat"] = "text/plain; charset=utf-8";
            typeProvider.Mappings[".sh"] = "text/plain; charset=utf-8";
            return typeProvider;
        }

        ////不准
        //public static Encoding GetFileEncoding(string srcFile)
        //{
        //    // *** Use Default of Encoding.Default (Ansi CodePage)
        //    Encoding enc = Encoding.Default;

        //    // *** Detect byte order mark if any - otherwise assume default
        //    byte[] buffer = new byte[5];
        //    FileStream file = new FileStream(srcFile, FileMode.Open);
        //    file.Read(buffer, 0, 5);
        //    file.Close();

        //    if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
        //        enc = Encoding.UTF8;

        //    else if (buffer[0] == 0xfe && buffer[1] == 0xff)
        //        enc = Encoding.Unicode;

        //    else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
        //        enc = Encoding.UTF32;

        //    //else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
        //    //    enc = Encoding.UTF7;

        //    return enc;
        //}
    }
}