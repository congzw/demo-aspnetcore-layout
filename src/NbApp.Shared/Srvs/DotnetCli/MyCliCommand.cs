using CliWrap;
using CliWrap.Buffered;
using System.Threading.Tasks;

namespace NbApp.Srvs.DotnetCli
{
    public class MyCliCommand
    {
        public static MyCliCommand Create(Command cmd)
        {
            var cmdWrap = new MyCliCommand() { Command = cmd };

            cmdWrap.Target = cmd.TargetFilePath;
            cmdWrap.Arguments = cmd.Arguments;
            cmdWrap.WorkingDirPath = cmd.WorkingDirPath;

            return cmdWrap;
        }
        internal Command Command { get; set; }

        public string Target { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirPath { get; set; }
        public int ExitCode { get; set; }
        public bool Success => ExitCode == 0;

        internal string StandardOutput { get; set; }
        internal string StandardError { get; set; }
        public string GetOutput()
        {
            return StandardOutput;
        }
        public string GetError()
        {
            return StandardError;
        }
    }

    public static class MyCliCommandExtensions
    {
        public static async Task<MyCliCommand> ExecuteBufferedAsync(this MyCliCommand cmdWrap, bool withValidationZeroExitCode)
        {
            var cmd = cmdWrap.Command;

            cmdWrap.Target = cmd.TargetFilePath;
            cmdWrap.Arguments = cmd.Arguments;
            cmdWrap.WorkingDirPath = cmd.WorkingDirPath;

            if (withValidationZeroExitCode)
            {
                cmd = cmd.WithValidation(CommandResultValidation.ZeroExitCode);
            }
            else
            {
                cmd = cmd.WithValidation(CommandResultValidation.None);
            }
            var cmdResult = await cmd.ExecuteBufferedAsync();

            // Result contains:
            // -- result.StandardOutput  (string)
            // -- result.StandardError   (string)
            // -- result.ExitCode        (int)
            // -- result.StartTime       (DateTimeOffset)
            // -- result.ExitTime        (DateTimeOffset)
            // -- result.RunTime         (TimeSpan)
            cmdWrap.StandardOutput = cmdResult.StandardOutput;
            cmdWrap.StandardError = cmdResult.StandardError;
            cmdWrap.ExitCode = cmdResult.ExitCode;

            return cmdWrap;
        }
    }
}