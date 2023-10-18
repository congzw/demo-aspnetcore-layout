using Microsoft.Extensions.DependencyInjection;
using NbApp.Web.Models;

namespace NbApp.Web.Bootstrap
{
    public static class SetupAppService
    {
        public static IServiceCollection AddTheAppService(IServiceCollection services)
        {
            services.AddSingleton<AppInfoVo>();
            return services;
        }
    }
}