using Microsoft.Extensions.DependencyInjection;

namespace NbApp.Web.Models
{
    public class TheAppSetting
    {
        public string AppName { get; set; } = "NbApp";
        public string PathBase { get; set; } = "";
        public bool DebugMode { get; set; } = true;
    }

    public static class TheAppSettingSetup
    {
        public static void Setup(IServiceCollection services)
        {
            services.AddOptions<TheAppSetting>().BindConfiguration("TheAppSetting");
        }
    }
}