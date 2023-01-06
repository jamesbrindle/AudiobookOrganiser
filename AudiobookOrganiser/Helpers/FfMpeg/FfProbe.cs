using AudiobookOrganiser;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace FfMpeg
{
    public class FfProbe
    {
        public static string GetChecksum(string inputPath)
        {
            try
            {
                string output = ExecuteProcessAndReadStdOut(inputPath);
                string[] parts = Regex.Split(output, "file checksum == ");
                string[] parts2 = Regex.Split(parts[1], @"\[");

                return parts2[0].Trim();
            }
            catch { }

            return string.Empty;
        }

        private static string ExecuteProcessAndReadStdOut(
            string arguments,
            int timeoutMs = 1000)
        {
            var _outputStringBuilder = new StringBuilder();

            var process = new Process();

            try
            {
                process.StartInfo.FileName = Program.FfProbePath;
                process.StartInfo.Arguments = $"-i \"{arguments}\"";
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
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            _outputStringBuilder.Append(e.Data);
                        }
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
                            {
                                _outputStringBuilder.Append(e.Data);
                            }
                        }
                    }

                    process.OutputDataReceived += outputHandler;
                    process.ErrorDataReceived += errorHandler;

                    try
                    {
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        if (process.WaitForExit(timeoutMs) && outputWaitHandle.WaitOne(timeoutMs) && errorWaitHandle.WaitOne(timeoutMs))
                        {
                            if (process.ExitCode != 0)
                            {
                                _outputStringBuilder.Append("Exit: " + process.ExitCode + Environment.NewLine +
                                 "Output from process: " + _outputStringBuilder.ToString());
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
                throw new Exception("Execution error: " + e.Message);
            }
            finally
            {
                process.Close();

                try
                {
                    process.Kill();
                }
                catch { }
            }

            return _outputStringBuilder.ToString();
        }
    }
}
