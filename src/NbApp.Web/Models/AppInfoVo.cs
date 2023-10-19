using System;
using System.Diagnostics;

namespace NbApp.Web.Models
{
    public class AppInfoVo
    {
        public string AppName { get; set; } = "NbApp";
        public bool DebugMode { get; set; } = true;
        public bool AutoLoadPageMenus { get; set; } = true;
    }
}