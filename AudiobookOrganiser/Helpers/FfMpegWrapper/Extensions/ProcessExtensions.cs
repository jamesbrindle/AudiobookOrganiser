using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.Extensions
{
    internal static class ProcessExtensions
    {
        public static int FfMpegProcessId { get; set; } = 0;

        internal static Task<int> WaitForExitAsync(
            this Process process,
            Action<int> onException,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var ctRegistration = new CancellationTokenRegistration();
            var mustUnregister = false;
            var tcs = new TaskCompletionSource<int>();
            if (cancellationToken != default(CancellationToken))
            {
                mustUnregister = true;
                ctRegistration = cancellationToken.Register(() =>
                {
                    try
                    {
                        // Send "q" to ffmpeg, which will force it to stop (closing files).
                        process.StandardInput.Write("q");
                    }
                    catch (InvalidOperationException)
                    {
                        // If the process doesn't exist anymore, ignore it.
                    }
                    finally
                    {
                        // Cancel the task. This will throw an exception to the calling program.
                        // Exc.Message will be "A task was canceled."
                        try
                        {
                            tcs.SetCanceled();
                        }
                        catch (Exception)
                        {
                        }
                    }
                });
            }

            void processOnExited(object sender, EventArgs e)
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                    onException?.Invoke(process.ExitCode);
                tcs.TrySetResult(process.ExitCode);
                if (mustUnregister) ctRegistration.Dispose();
                process.Exited -= processOnExited;
            }

            process.EnableRaisingEvents = true;
            process.Exited += processOnExited;

            var started = process.Start();
            if (!started)
                tcs.TrySetException(new InvalidOperationException($"Could not start process {process}"));

            process.BeginErrorReadLine();

            return tcs.Task;
        }

        internal static string ExecuteProcessAndReadStdOut(
            string filePath,
            string arguments,
            int timeoutMs = 10000)
        {
            var _outputStringBuilder = new StringBuilder();

            var process = new Process();

            try
            {
                process.StartInfo.FileName = filePath;
                process.StartInfo.Arguments = arguments;
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