﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Argus.Calibration.Helper
{
    public static class ShellHelper
    {
        public static Task<int> Bash(this string cmd, Action action = null)
        {
            var source = new TaskCompletionSource<int>();
            var escapedArgs = cmd.Replace("\"", "\\\"");
            escapedArgs = escapedArgs.Replace(@"\", @"\\");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };
            
            process.Exited += (sender, args) =>
            {
                if (action != null)
                {
                    action();
                }

                Trace.WriteLine(process.StandardError.ReadToEnd());
                Trace.WriteLine(process.StandardOutput.ReadToEnd());
                if (process.ExitCode == 0)
                {
                    source.SetResult(0);
                }
                else
                {
                    source.SetException(new Exception($"Command `{cmd}` failed with exit code `{process.ExitCode}`"));
                }

                process.Dispose();
            };

            try
            {
                var start = process.Start();
            }
            catch (Exception e)
            {
                source.SetException(e);
            }

            return source.Task;
        }

        public static int RunSync(this string cmd)
        {
            var source = new TaskCompletionSource<int>();
            var escapedArgs = cmd.Replace("\"", "\\\"");
            escapedArgs = escapedArgs.Replace(@"\", @"\\");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            var start = process.Start();

            process.WaitForExit();

            Trace.WriteLine(process.StandardOutput.ReadToEnd());
            Trace.WriteLine(process.StandardError.ReadToEnd());

            return process.ExitCode;
        }

        public static Process RunKillable(this string cmd, Action action = null)
        {
            var source = new TaskCompletionSource<int>();
            var escapedArgs = cmd.Replace("\"", "\\\"");
            escapedArgs = escapedArgs.Replace(@"\", @"\\");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
            {
                if (action != null)
                {
                    action();
                }

                Trace.WriteLine(process.StandardError.ReadToEnd());
                Trace.WriteLine(process.StandardOutput.ReadToEnd());
                if (process.ExitCode == 0)
                {
                    source.SetResult(0);
                }
                else
                {
                    source.SetException(new Exception($"Command `{cmd}` failed with exit code `{process.ExitCode}`"));
                }

                process.Dispose();
            };

            process.Start();

            process.WaitForExitAsync();

            return process;
        }
    }
}