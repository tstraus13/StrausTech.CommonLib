using System.Text;
using System.Diagnostics;

namespace StrausTech.CommonLib;

public static class LocalSystem
{
    public static class Linux
    {
        public static ProcessResult Execute(string command, string workingDir = "", Dictionary<string, string>? env = null)
        {
            var processInfo = new ProcessStartInfo("/bin/bash", $"-c \"{command.Replace("\"", "\\\"")}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = !string.IsNullOrEmpty(workingDir) ? workingDir : ""
            };

            if (env != null)    
                foreach (var var in env)
                    processInfo.Environment.Add(var.Key, var.Value);

            var process = Process.Start(processInfo);

            if (process == null)
                return new ProcessResult
                {
                    ExitCode = 1,
                    StdErr = "An Error occurred trying to execute the command",
                    StdOut = ""
                };

            process.WaitForExit();

            var result = new ProcessResult
            {
                ExitCode = process.ExitCode,
                StdErr = process.StandardError.ReadToEnd(),
                StdOut = process.StandardOutput.ReadToEnd()
            };

            process.Close();

            return result;
        }

        public static Process? ExecuteBackground(string command, string workingDir = "", Dictionary<string, string>? env = null)
        {
            var processInfo = new ProcessStartInfo("/bin/bash", $"-c \"{command.Replace("\"", "\\\"")}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = !string.IsNullOrEmpty(workingDir) ? workingDir : ""
            };

            if (env != null)    
                foreach (var var in env)
                    processInfo.Environment.Add(var.Key, var.Value);

            var process = Process.Start(processInfo);

            return process;
        }
    }

    public static class Windows
    {
        // TODO: Implement Executing Windows
    }

    public static class MacOS
    {
        // TODO: Implement Executing MacOS
    }

    public class ProcessResult
    {
        public int ExitCode { get; set; }

        public string StdOut { get; set; } = "";

        public string StdErr { get; set; } = "";
    }
}