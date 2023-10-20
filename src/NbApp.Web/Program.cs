using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NbApp.Srvs.MdDocs;
using NbApp.Web.Bootstrap;
using NbApp.Web.Models;
using System;
using System.Threading.Tasks;

namespace NbApp.Web
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("==== args ====");
                foreach (var item in args)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine();
            }

            var builder = WebApplication.CreateBuilder(args);


            SetupAppService.AddTheAppService(builder.Services);

            builder.Services
                .AddRazorPages(options => {
                    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
                })
                .AddRazorRuntimeCompilation();

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            builder.Services.AddTransient<MdDocFileService>();
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var appInfoVo = scope.ServiceProvider.GetService<AppInfoVo>();
                if (!string.IsNullOrWhiteSpace(appInfoVo.PathBase) || appInfoVo.PathBase != "/")
                {
                    app.UsePathBase(appInfoVo.PathBase);
                }
            }

            app.UseDeveloperExceptionPage();
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //}

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapRazorPages();

            await app.RunAsync();
        }
    }
}