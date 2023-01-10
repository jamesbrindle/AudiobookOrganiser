using System.Linq;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.Helpers
{
    public static class MediaFileTypes
    {
        public static string[] Video
        {
            get
            {
                string[] ext =
                {
                    ".swf", ".mov", ".mp4", ".m4v", ".3gp", ".3g2", ".flv", ".f4v", ".avi", ".mpgeg", ".mpg", ".wmv",
                    ".asf", ".ram", ".mkv", "webm"
                };
                var extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Audio
        {
            get
            {
                string[] ext =
                {
                    ".aax", ".mp3", ".wav", ".aif", ".aiff", ".mpa",".wma", ".3gp", ".aac", ".acc", ".aa", ".act",
                    ".aiff", ".alac", ".amr", ".ape",
                    ".au", ".awb", ".dss", ".dvf", ".flac", ".ivs", ".m4b", ".m4p", ".mmf", ".mpc", ".nsf",
                    ".oof", ".oga", ".mogg", ".m4a", ".m4b",
                    ".opus", ".ra", ".rm", ".raw", ".rf64", "sln", ".tta", ".voc", ".vox", "8svx", ".cda"
                };
                var extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }
    }
}



