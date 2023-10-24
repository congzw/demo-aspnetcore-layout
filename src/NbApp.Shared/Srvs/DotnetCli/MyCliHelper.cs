using CliWrap;
using CliWrap.Buffered;
using System;
using System.Collections.Generic;
using System.Linq;
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
        internal List<string> Arguments { get; set; } = new List<string>();

        public MyCliCommand WithArguments(params string[] arguments)
        {
            var trimArgs = arguments.Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (trimArgs.Length > 0)
            {
                Command = Command.WithArguments(trimArgs);
                Arguments.AddRange(trimArgs);
            }
            return this;
        }

        public static MyCliCommand Create(Command cmd)
        {
            return MyCliHelper.Instance.Create(cmd);
        }
    }

    public class MyCliCommandResult
    {
        public string Target { get; set; }
        public string Arguments { get; set; }
        public List<string> ArgumentItems { get; set; } = new List<string>();
        public string WorkingDirPath { get; set; }
        public int ExitCode { get; set; }
        public bool Success { get; set; }
        public string Output { get; set; }
    }

    public static class MyCliCommandExtensions
    {
        public static string[] ParseToArgumentsArray(this MyCliHelper helper, string argumentsValue, char separator)
        {
            if (string.IsNullOrWhiteSpace(argumentsValue))
            {
                return Array.Empty<string>();
            }
            var arguments = argumentsValue.Split(separator).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
            return arguments;
        }

        //public static string ParseToArgumentsValue(this MyCliHelper helper, string[] arguments, char separator = ' ')
        //{
        //    if (arguments == null || arguments.Length == 0)
        //    {
        //        return "";
        //    }
        //    return string.Join(separator, arguments.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray());
        //}

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
            return MyCliCommand.Create(Cli.Wrap(targetFile)).WithArguments(arguments);
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

            var myResult = new MyCliCommandResult();
            myResult.Target = cmdWrap.Command.TargetFilePath;
            myResult.Arguments = cmdWrap.Command.Arguments;
            myResult.ArgumentItems = cmdWrap.Arguments;
            myResult.WorkingDirPath = cmdWrap.Command.WorkingDirPath;

            if (withValidationZeroExitCode)
            {
                cmdWrap.Command = cmdWrap.Command.WithValidation(CommandResultValidation.ZeroExitCode);
            }
            else
            {
                cmdWrap.Command = cmdWrap.Command.WithValidation(CommandResultValidation.None);
            }

            try
            {
                var cmdResult = await cmdWrap.Command.ExecuteBufferedAsync();
                myResult.ExitCode = cmdResult.ExitCode;
                myResult.Success = cmdResult.ExitCode == 0 && string.IsNullOrWhiteSpace(cmdResult.StandardError);
                myResult.Output = "";
                if (!string.IsNullOrWhiteSpace(cmdResult.StandardOutput))
                {
                    myResult.Output += cmdResult.StandardOutput;
                }
                if (!string.IsNullOrWhiteSpace(cmdResult.StandardError))
                {
                    myResult.Output += (Environment.NewLine + cmdResult.StandardError);
                }
            }
            catch (Exception ex)
            {
                myResult.ExitCode = -1;
                myResult.Output = ex.ToString();
            }

            return myResult;
        }
    }
}