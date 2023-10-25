using System.Collections.Generic;

namespace NbApp.Web.Models
{
    public class SystemdServiceVo
    {
        public string srv_dir { get; set; }
        public string srv_name { get; set; }
        public string user { get; set; } = "a";
        public string restart_type { get; set; } = "always";
        public int restart_sec { get; set; } = 15;
        public List<string> srvice_exts { get; set; } = new List<string>() { "KillSignal=SIGINT", "Environment=DOTNET_ROOT=/opt/media/dotnet/" };

        public static SystemdServiceVo Create(string srv_dir, string srv_name)
        {
            var item = new SystemdServiceVo();
            item.srv_dir = srv_dir;
            item.srv_name = srv_name;
            return item;
        }
    }
}