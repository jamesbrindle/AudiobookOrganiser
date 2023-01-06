using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace AudiobookOrganiser.Helpers
{
    internal class ProcessHelper
    {
        public static string ExecuteProcessAndReadStdOut(
            string path,
            out string errorOutput,
            string arguments = "",
            string workingDirectory = "",
            int timeoutSeconds = 60,
            bool throwOnError = true
        )
        {
            var _outputStringBuilder = new StringBuilder();
            var _errorStringBuilder = new StringBuilder();
            int timeoutInMs = timeoutSeconds * 1000;

            var process = new Process();

            try
            {
                process.StartInfo.FileName = path;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WorkingDirectory = string.IsNullOrEmpty(workingDirectory)
                    ? Path.GetDirectoryName(path)
                    : workingDirectory;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;

                using (var outputWaitHandle = new AutoResetEvent(false))
                using (var errorWaitHandle = new AutoResetEvent(false))
                {
                    void outputHandler(object sender, System.Diagnostics.DataReceivedEventArgs e)
                    {
                        if (e.Data == null)
                            outputWaitHandle.Set();
                        else
                            _outputStringBuilder.Append(e.Data);
                    }

                    void errorHandler(object sender, System.Diagnostics.DataReceivedEventArgs e)
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(e.Data) && !e.Data.Contains("diacritics"))
                                _outputStringBuilder.Append(e.Data);
                        }
                    }

                    process.OutputDataReceived += outputHandler;
                    process.ErrorDataReceived += errorHandler;

                    try
                    {
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        if (process.WaitForExit(timeoutInMs) &&
                            outputWaitHandle.WaitOne(timeoutInMs) &&
                            errorWaitHandle.WaitOne(timeoutInMs))
                        {
                            if (process.ExitCode != 0)
                            {
                                string output = _outputStringBuilder.ToString();
                                throw new Exception("Exit: " + process.ExitCode + Environment.NewLine +
                                                    "Output from process: " + _outputStringBuilder);
                            }
                        }
                        else
                        {
                            if (!process.HasExited)
                            {
                                process.Kill();
                                throw new Exception("ERROR: Process took too long to finish");
                            }
                        }
                    }
                    finally
                    {
                        process.OutputDataReceived -= outputHandler;
                        process.ErrorDataReceived -= errorHandler;
                    }
                }
            }
            catch (Exception e)
            {
                if (throwOnError)
                    throw new Exception("Execution error: " + e.Message);
            }
            finally
            {
                process.Close();

                try
                {
                    process.Kill();
                }
                catch
                {
                }
            }

            errorOutput = _errorStringBuilder.ToString();
            return _outputStringBuilder.ToString();
        }
    }
}
