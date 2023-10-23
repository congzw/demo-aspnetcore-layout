using CliWrap;
using CliWrap.Buffered;
using System.Threading.Tasks;

namespace NbApp.Srvs.DotnetCli
{
    public class MyCliHelper
    {
        public static MyCliHelper Instance = new MyCliHelper();

        public MyCliCommand Create(Command cmd)
        {
            var cmdWrap = new MyCliCommand() { Command = cmd };
            return cmdWrap;
        }
    }

    public class MyCliCommand
    {
        internal Command Command { get; set; }

        public static MyCliCommand Create(Command cmd)
        {
            return MyCliHelper.Instance.Create(cmd);
        }
    }

    public class MyCliCommandResult
    {
        public string Target { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirPath { get; set; }
        public int ExitCode { get; set; }
        public bool Success => ExitCode == 0;
        public string Output { get; set; }
    }

    public static class MyCliCommandExtensions
    {
        public static MyCliCommand CreateCommand(this MyCliHelper helper, string targetFile, params string[] arguments)
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

        public static async Task<MyCliCommandResult> ExecuteBufferedAsync(this MyCliCommand cmdWrap, bool withValidationZeroExitCode)
        {
            // Result contains:
            // -- result.StandardOutput  (string)
            // -- result.StandardError   (string)
            // -- result.ExitCode        (int)
            // -- result.StartTime       (DateTimeOffset)
            // -- result.ExitTime        (DateTimeOffset)
            // -- result.RunTime         (TimeSpan)

            var cmd = cmdWrap.Command;
            var myResult = new MyCliCommandResult();
            myResult.Target = cmd.TargetFilePath;
            myResult.Arguments = cmd.Arguments;
            myResult.WorkingDirPath = cmd.WorkingDirPath;

            if (withValidationZeroExitCode)
            {
                cmdWrap.Command = cmd.WithValidation(CommandResultValidation.ZeroExitCode);
            }
            else
            {
                cmdWrap.Command = cmd.WithValidation(CommandResultValidation.None);
            }
            var cmdResult = await cmd.ExecuteBufferedAsync();

            myResult.ExitCode = cmdResult.ExitCode;
            myResult.Output = myResult.Success ? cmdResult.StandardOutput : cmdResult.StandardError;

            return myResult;
        }
    }
}