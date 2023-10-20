using Common;
using Microsoft.Extensions.DependencyInjection;
using NbApp.Web.Models;

namespace NbApp.Web.Bootstrap
{
    public static class SetupAppService
    {
        public static IServiceCollection AddTheAppService(IServiceCollection services)
        {
            WebRunInfo.Instance.Setup(services);

            //todo: load from setting
            services.AddSingleton<AppInfoVo>();
            return services;
        }
    }
}