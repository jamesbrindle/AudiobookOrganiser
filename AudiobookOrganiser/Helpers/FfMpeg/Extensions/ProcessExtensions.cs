using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FfMpeg.Extensions
{
    public static class ProcessExtensions
    {
        public static int FfMpegProcessId { get; set; } = 0;

        public static Task<int> WaitForExitAsync(
            this Process process,
            Action<int> onException,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var ctRegistration = new CancellationTokenRegistration();
            bool mustUnregister = false;
            var tcs = new TaskCompletionSource<int>();
            if (cancellationToken != default(CancellationToken))
            {
                mustUnregister = true;
                ctRegistration = cancellationToken.Register(() =>
                {
                    try
                    {
                        process.StandardInput.Write("q");
                    }
                    catch (InvalidOperationException)
                    { }
                    finally
                    {
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

            bool started = process.Start();
            FfMpegProcessId = process.Id;

            if (!started)
                tcs.TrySetException(new InvalidOperationException($"Could not start process {process}"));

            process.BeginErrorReadLine();

            return tcs.Task;
        }
    }
}