using System.Collections.Generic;

namespace NbApp.Web.Models
{
    public class SystemdTemplateVo
    {
        public string my_srv_dir { get; set; }
        public string my_srv_name { get; set; }
        public string my_user { get; set; } = "a";
        public string my_restart_type { get; set; } = "always";
        public int my_restart_sec { get; set; } = 15;
        public List<string> my_srvice_exts { get; set; } = new List<string>() { "KillSignal=SIGINT", "Environment=DOTNET_ROOT=/opt/media/dotnet/" };

        public static SystemdTemplateVo Create(string my_srv_dir, string my_srv_name)
        {
            var item = new SystemdTemplateVo();
            item.my_srv_dir = my_srv_dir;
            item.my_srv_name = my_srv_name;
            return item;
        }
    }
}