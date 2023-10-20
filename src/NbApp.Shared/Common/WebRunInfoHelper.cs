using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using System.IO;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class WebRunInfo
    {
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public string ContentRootPath { get; set; }
        public string WebRootPath { get; set; }
        public string ApplicationName { get; set; }
        public string EnvironmentName { get; set; }

        public string ProcessPath => Environment.ProcessPath;
        public string CommandLine => Environment.CommandLine;
        public Version RuntimeVersion => Environment.Version;
        public IDictionary GetEnvironmentVariables() => Environment.GetEnvironmentVariables();

        public string CurrentDirectory => Environment.CurrentDirectory;
        public int ProcessId => Environment.ProcessId;
        public string[] CommandLineArgs => Environment.GetCommandLineArgs();

        public TimeSpan SystemStartDuration => TimeSpan.FromMilliseconds(Environment.TickCount);
        public TimeSpan AppStartDuration => DateTime.Now - AppStartAt;
        public DateTime AppStartAt => CurrentProcess.StartTime;
        internal Process CurrentProcess => Process.GetCurrentProcess();
        public bool IsRunningInAzure => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
        public string LocalAppDataDir => GetLocalApplicationDataDir(Assembly.GetEntryAssembly().GetName().Name);
        public string GetLocalApplicationDataDir(string appName)
        {
            if (IsRunningInAzure)
            {
                // If running in azure, we won't use local app folder as its temporary and will frequently be deleted.
                // Use home folder instead.
                return Path.Combine(Path.GetFullPath("/home"), appName);
            }
            else
            {
                var localAppFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(localAppFolder, appName);
            }
        }

        public IDictionary<string, object> Bags { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public WebRunInfo Refresh(IServiceProvider sp)
        {
            if (sp == null)
            {
                throw new Exception("sp or SimpleServiceLocator should be provided at least one!");
            }

            var webEnv = sp.GetService<IWebHostEnvironment>();
            this.ContentRootPath = webEnv.ContentRootPath;
            this.WebRootPath = webEnv.WebRootPath;
            this.ApplicationName = webEnv.ApplicationName;
            this.EnvironmentName = webEnv.EnvironmentName;
            this.UpdateAt = DateTime.Now;

            return this;
        }

        public static WebRunInfo Instance = new WebRunInfo();
    }

    #region setup appUrls

    public static class WebRunInfoExtensions
    {
        public static void Setup(this WebRunInfo instance, IServiceCollection services)
        {
            services.AddSingleton(instance);
        }
    }

    #endregion
}
