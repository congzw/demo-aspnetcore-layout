using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class AssemblySpec
    {
        public string AssemblyVersion { get; set; }
        public string FileVersion { get; set; }
        public string InformationalVersion { get; set; }

        public string Company { get; set; }
        public string Title { get; set; }
        public string Copyright { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }
        public string Configuration { get; set; }

        public static AssemblySpec Create(Assembly assembly)
        {
            #region infos

            //[assembly: System.Reflection.AssemblyCompanyAttribute("some_company")]
            //[assembly: System.Reflection.AssemblyProductAttribute("some_product")]
            //[assembly: System.Reflection.AssemblyCopyrightAttribute("some_copyright")]
            //[assembly: System.Reflection.AssemblyTitleAttribute("some_title")]
            //[assembly: System.Reflection.AssemblyConfigurationAttribute("Debug")]

            //[assembly: System.Reflection.AssemblyVersionAttribute("1.2.0")]
            //[assembly: System.Reflection.AssemblyFileVersionAttribute("1.3.0")]
            //[assembly: System.Reflection.AssemblyInformationalVersionAttribute("This is a prerelease package")]

            #endregion

            var spec = new AssemblySpec();

            spec.Company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
            spec.Product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
            spec.Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
            spec.Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
            spec.Configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;
            spec.Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;

            spec.AssemblyVersion = assembly.GetName().Version.ToString();
            spec.FileVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            spec.InformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            return spec;
        }

        public static AssemblySpec GetAssemblySpec(Assembly assembly = null, bool forceRead = false)
        {
            if (assembly == null)
            {
                return AssemblySpecHelper.Instance.GetAssemblySpec(forceRead);
            }
            return AssemblySpecHelper.Instance.GetAssemblySpec(assembly);
        }
    }

    #region helper

    internal class AssemblySpecHelper
    {
        internal static AssemblySpecHelper Instance = new AssemblySpecHelper();
        internal Dictionary<string, AssemblySpec> CacheItems { get; set; } = new Dictionary<string, AssemblySpec>(StringComparer.OrdinalIgnoreCase);
        internal AssemblySpec GetAssemblySpec(Assembly assembly, bool forceRead = false)
        {
            if (assembly == null)
            {
                return null;
            }

            var theKey = assembly.FullName;
            if (CacheItems.TryGetValue(theKey, out var spec) && !forceRead)
            {
                return spec;
            }

            spec = AssemblySpec.Create(assembly);
            CacheItems[theKey] = spec;
            return spec;
        }
        internal AssemblySpec GetAssemblySpec(bool forceRead = false)
        {
            return GetAssemblySpec(Assembly.GetEntryAssembly(), forceRead);
        }
    }

    #endregion
}