using System.Collections.Generic;

namespace NbApp.Web.Models
{
    public class AppInfoVo
    {
        public string AppName { get; set; } = "NbApp";
        public string PathBase { get; set; } = "";
        public bool DebugMode { get; set; } = true;
    }
}