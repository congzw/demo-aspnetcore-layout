using CliWrap;

namespace NbApp.Srvs.DotnetCli
{
    public class DotnetCliWrap
    {
        public static DotnetCliWrap Instance = new DotnetCliWrap();
    }

    public static class DotnetCliHelperExtensions
    {
        //public static MyCliCommand GetBadCommand(this DotnetCliWrap helper)
        //{
        //    var cmd = Cli.Wrap("dotnet")
        //     .WithArguments("--bad-command");
        //    return MyCliCommand.Create(cmd);
        //}

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

        public static MyCliCommand CreateCommand(this DotnetCliWrap helper, string targetFile, params string[] arguments)
        {
            // Equivalent to: `git commit -m "my commit"`
            //.WithArguments(new[] { "commit", "-m", "my commit" });

            // Each Add(...) call takes care of formatting automatically.
            // Equivalent to: `git clone some_url --depth 20`
            //.WithArguments(args => args
            //    .Add("clone")
            //    .Add(some_url)
            //    .Add("--depth")
            //    .Add(20)
            //);

            var cmd = Cli.Wrap(targetFile)
                .WithArguments(arguments);
            return MyCliCommand.Create(cmd);
        }

    }
}