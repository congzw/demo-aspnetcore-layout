using CliWrap;

namespace NbApp.Srvs.DotnetCli
{
    public class DotnetCliWrap
    {
        public static DotnetCliWrap Instance = new DotnetCliWrap();
    }

    public static class DotnetCliHelperExtensions
    {
        public static MyCliCommand GetHelp(this DotnetCliWrap helper)
        {
            var cmd = Cli.Wrap("dotnet")
             .WithArguments("--help");
            return MyCliCommand.Create(cmd);
        }

        public static MyCliCommand GetVersion(this DotnetCliWrap helper)
        {
            var cmd = Cli.Wrap("dotnet")
             .WithArguments("--version");
            return MyCliCommand.Create(cmd);
        }

        public static MyCliCommand GetInfo(this DotnetCliWrap helper)
        {
            var cmd = Cli.Wrap("dotnet")
             .WithArguments("--info");
            return MyCliCommand.Create(cmd);
        }

        public static MyCliCommand GetSdks(this DotnetCliWrap helper)
        {
            var cmd = Cli.Wrap("dotnet")
             .WithArguments("--list-sdks");
            return MyCliCommand.Create(cmd);
        }

        public static MyCliCommand GetRuntimes(this DotnetCliWrap helper)
        {
            var cmd = Cli.Wrap("dotnet")
             .WithArguments("--list-runtimes");
            return MyCliCommand.Create(cmd);
        }
    }
}