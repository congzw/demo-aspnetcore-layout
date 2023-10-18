using System.Collections.Generic;

namespace NbApp.Web.Models
{
    public class AutoRunSystemdVo
    {
        public string my_srv_dir { get; set; }
        public string my_srv_name { get; set; }
        public string my_srv_desc { get; set; }
        public string my_user { get; set; }
        public string my_restart_type { get; set; }
        public int my_restart_sec { get; set; }
        public List<int> my_ports { get; set; } = new List<int>();
    }
}