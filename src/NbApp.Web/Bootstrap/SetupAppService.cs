using Common;
using Microsoft.Extensions.DependencyInjection;
using NbApp.Srvs.DotnetCli;
using NbApp.Web.Models;

namespace NbApp.Web.Bootstrap
{
    public static class SetupAppService
    {
        public static IServiceCollection AddTheAppService(IServiceCollection services)
        {
            TheAppSettingSetup.Setup(services);
            WebRunInfo.Instance.Setup(services);
            return services;
        }
    }
}