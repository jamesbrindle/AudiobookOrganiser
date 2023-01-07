using System;
using System.Text;
using System.Threading;


namespace AudiobookOrganiser.Helpers
{
    /// <summary>
    /// An ASCII progress bar
    /// 
    /// Use like:
    /// 
    ///   Console.Write("Performing some task... ");
    ///   using (var progress = new ProgressBar())
    ///   {
    ///       for (int i = 0; i <= 100; i++)
    ///       {
    ///           progress.Report((double) i / 100);
    ///           Thread.Sleep(500);
    ///       }
    ///   }
    ///   Console.WriteLine("Done.");
    /// 
    /// </summary>
    public class ProgressBar : IDisposable, IProgress<double>
    {
        private const int blockCount = 10;
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private const string animation = @"|/-\";

        private readonly Timer timer = null;

        private double currentProgress = 0;
        private string currentText = string.Empty;
        private bool disposed = false;
        private int animationIndex = 0;

        public ProgressBar()
        {
            timer = new Timer(TimerHandler);

            // A progress bar is only for temporary display in a console window.
            // If the console output is redirected to a file, draw nothing.
            // Otherwise, we'll end up with a lot of garbage in the target file.
            if (!Console.IsOutputRedirected)
            {
                ResetTimer();
            }
        }

        public void Report(double value)
        {
            // Make sure value is in [0..1] range
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref currentProgress, value);
        }

        private void TimerHandler(object state)
        {
            if (disposed)
                return;

            int progressBlockCount = (int)(currentProgress * blockCount);
            double percent = (double)(currentProgress * 100);

            string text = string.Format("[{0}{1}] {2:0.0}% {3}",
                new string('#', progressBlockCount),
                new string('-', blockCount - progressBlockCount),
                percent,
                (percent != 100 ? animation[animationIndex++ % animation.Length] : '\0'));
            UpdateText(text);

            ResetTimer();
        }

        private void UpdateText(string text)
        {
            Console.CursorVisible = false;

            // Get length of common portion
            int commonPrefixLength = 0;
            int commonLength = Math.Min(currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == currentText[commonPrefixLength])
                commonPrefixLength++;

            // Backtrack to the first differing character
            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', currentText.Length - commonPrefixLength);

            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));

            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Write(outputBuilder);
            currentText = text;
        }

        private void ResetTimer()
        {
            timer.Change(animationInterval, TimeSpan.FromMilliseconds(-1));
        }

        public void Dispose()
        {
            try
            {
                if (timer != null)
                    timer.Dispose();
            }
            catch { }

            disposed = true;
            UpdateText(string.Empty);
        }
    }
}