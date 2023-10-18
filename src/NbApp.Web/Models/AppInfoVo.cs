using System;
using System.Diagnostics;

namespace NbApp.Web.Models
{
    public class AppInfoVo
    {
        public string AppName { get; set; } = "NbApp";

        internal Process CurrentProcess => Process.GetCurrentProcess();
        public DateTime AppStartAt => CurrentProcess.StartTime;
        public TimeSpan AppStartDuration => DateTime.Now - AppStartAt;
        
        public int ProcessId => Environment.ProcessId;
        public string[] CommandLineArgs => Environment.GetCommandLineArgs();
        public string CurrentDirectory => Environment.CurrentDirectory;

        public bool DebugMode { get; set; } = true;
    }
}