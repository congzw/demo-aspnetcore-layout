using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NbApp.Web.Models
{
    public class SystemdSetting
    {
        public string my_srv_dir { get; set; }
        public string my_srv_name { get; set; }
        public string my_srv_file_name { get; set; }
        public string my_exe_start { get; set; }

        public string my_srv_desc { get; set; }
        public string my_user { get; set; }
        public string my_restart_type { get; set; }
        public int my_restart_sec { get; set; }
        public List<int> my_ports { get; set; } = new List<int>();
        public List<string> my_srvice_exts { get; set; } = new List<string>();

        public string my_copy_from { get; set; }
        public string my_copy_to { get; set; }

        internal string EntryAssemblyLocation { get; set; }

        public static SystemdSetting CreateDefault()
        {
            var assm = Assembly.GetEntryAssembly();
            var assmLocation = assm.Location;

            var item = new SystemdSetting();
            item.EntryAssemblyLocation = assmLocation;
            item.my_srv_dir = Path.GetDirectoryName(assmLocation);
            item.my_srv_name = Path.GetFileNameWithoutExtension(assmLocation);
            item.my_exe_start = Path.Combine(item.my_srv_dir, item.my_srv_name);

            item.my_srv_file_name = $"{item.my_srv_name}.service";
            item.my_srv_desc = $"{item.my_srv_name}";
            item.my_user = "a";
            item.my_restart_type = "always";
            item.my_restart_sec = 15;
            item.my_ports.Add(10001);

            item.my_srvice_exts.Add("KillSignal=SIGINT");
            item.my_srvice_exts.Add("Environment=DOTNET_ROOT=/opt/media/dotnet/");

            var saveTo = Path.Combine(item.my_srv_dir, "_autorun", "linux-x64", item.my_srv_file_name);
            item.my_copy_from = saveTo;

            var rootDir = new DirectoryInfo("/");
            var copyTo = Path.Combine(rootDir.FullName, "ect", "systemd", "system", item.my_srv_file_name);
            item.my_copy_to = copyTo;
            return item;
        }
    }
}