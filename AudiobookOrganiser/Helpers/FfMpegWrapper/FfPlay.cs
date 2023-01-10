using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    /// <summary>
    /// FFPlay will play media (audio or video) in a basic window with a very limited AI. Only included here 
    /// for the scenario of playing 'encrypted' files (i.e. AAX, AAXC files)
    /// </summary>
    public class FfPlay
    {
        private readonly string _ffPlayExePath = string.Empty;

        /// <summary>
        /// FFPlay will play media (audio or video) in a basic window with a very limited AI. Only included here 
        /// for the scenario of playing 'encrypted' files (i.e. AAX, AAXC files)
        /// </summary>
        /// <param name="ffPlayExePath">Path to ffplay.exe</param>
        public FfPlay(string ffPlayExePath)
        {
            _ffPlayExePath = ffPlayExePath;
        }

        /// <summary>
        /// Play media (audio or video) in a basic window with a very limited AI. Only included here 
        /// for the scenario of playing 'encrypted' files (i.e. AAX, AAXC files)
        /// </summary>
        /// <param name="mediaFilePath">Full path to audio or video file</param>
        /// <param name="showInstructionDialog">Optionally show a dialogue with the UI intructions (since it's not obvious)</param>
        public void PlayMedia(string mediaFilePath, bool showInstructionDialog = false)
        {
            var ffPlayProcess = new Process();

            ffPlayProcess.StartInfo.RedirectStandardOutput = true;
            ffPlayProcess.StartInfo.UseShellExecute = false;
            ffPlayProcess.StartInfo.CreateNoWindow = true;
            ffPlayProcess.StartInfo.FileName = _ffPlayExePath;
            ffPlayProcess.StartInfo.Arguments = "\"" + mediaFilePath + "\"";

            ffPlayProcess.Start();

            if (showInstructionDialog)
                ShowInstructionsDialogue();
        }

        /// <summary>
        /// Play media (audio or video) in a basic window with a very limited AI. Only included here 
        /// for the scenario of playing 'encrypted' files (i.e. AAX, AAXC files)
        /// </summary>
        /// <param name="mediaFilePath">Full path to audio or video file</param>
        /// <param name="showInstructionDialog">Optionally show a dialogue with the UI intructions (since it's not obvious)</param>
        public void PlayMedia(string mediiaFilePath, string aaxActivationBytes, bool showInstructionDialog = false)
        {
            var ffPlayProcess = new Process();

            ffPlayProcess.StartInfo.RedirectStandardOutput = true;
            ffPlayProcess.StartInfo.UseShellExecute = false;
            ffPlayProcess.StartInfo.CreateNoWindow = true;
            ffPlayProcess.StartInfo.FileName = _ffPlayExePath;
            ffPlayProcess.StartInfo.Arguments =
                (string.IsNullOrEmpty(aaxActivationBytes)
                    ? ""
                    : "-activation_bytes " + aaxActivationBytes + " ")
                + "\"" + mediiaFilePath + "\"";

            ffPlayProcess.Start();

            if (showInstructionDialog)
                ShowInstructionsDialogue();
        }

        /// <summary>
        /// Play media (audio or video) in a basic window with a very limited AI. Only included here 
        /// for the scenario of playing 'encrypted' files (i.e. AAX, AAXC files)
        /// </summary>
        /// <param name="mediaFilePath">Full path to audio or video file</param>
        /// <param name="showInstructionDialog">Optionally show a dialogue with the UI intructions (since it's not obvious)</param>
        public void PlayMedia(string mediaFilePath, string aaxcActivationKey, string aaxcActivationIv, bool showInstructionDialog = false)
        {
            var ffPlayProcess = new Process();

            ffPlayProcess.StartInfo.RedirectStandardOutput = true;
            ffPlayProcess.StartInfo.UseShellExecute = false;
            ffPlayProcess.StartInfo.CreateNoWindow = true;
            ffPlayProcess.StartInfo.FileName = _ffPlayExePath;
            ffPlayProcess.StartInfo.Arguments =
                (string.IsNullOrEmpty(aaxcActivationKey) || string.IsNullOrEmpty(aaxcActivationIv)
                    ? ""
                    : "-audible_key " + aaxcActivationKey + " " + "-audible_iv " + aaxcActivationIv + " ")
                + "\"" + mediaFilePath + "\"";

            ffPlayProcess.Start();

            if (showInstructionDialog)
                ShowInstructionsDialogue();
        }

        private static void ShowInstructionsDialogue()
        {
            new Thread((ThreadStart)delegate
            {
                Thread.Sleep(1500);
                MessageBox.Show(
                    "Control Keys:\n\n" +
                    "S - Step to the next frame, pause.\n" +
                    "Left / Right - Seek backwards/forwards 10 seconds.\n" +
                    "Down / Up - Seek backwards/forwards 1 minute.\n" +
                    "Page Down / Page Up - Seek to the previous/next chapter. If no chapters, seek 10 minutes.\n\n" +
                    "Mouse Control:\n\n" +
                    "Right Mouse Click - Seek to percentage in file corresponding to fraction of width.",
                    "ffplay Instructions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }).Start();
        }
    }
}
