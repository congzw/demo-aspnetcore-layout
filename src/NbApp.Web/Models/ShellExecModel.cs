namespace NbApp.Web.Models
{
    public class ShellExecModel
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public bool? Success { get; set; }

        public static ShellExecModel Create(string input, string output, bool? success = null)
        {
            return new ShellExecModel() { Input = input, Output = output, Success = success };
        }
    }
}