using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.AudibleActivationBytesHelper.Lib.Ex
{
    [Obfuscation(Exclude = true)]
    static class HtmlDecode
    {
        public static string Decode(this string s)
        {
            if (s is null)
                return null;
            return WebUtility.HtmlDecode(s);
        }

        public static string[] Decode(this string[] ss)
        {
            if (ss is null)
                return new string[0];
            return ss.Select(Decode).ToArray();
        }
    }

    [Obfuscation(Exclude = true)]
    public static class EReducedBitRateEx
    {
        private static readonly Regex _rgxEnumBitrate = new Regex(@"(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static uint UInt32(this EReducedBitRate ebitrate)
        {
            string sbitrate = ebitrate.ToString();
            Match match = _rgxEnumBitrate.Match(sbitrate);
            if (match.Success)
            {
                if (uint.TryParse(match.Groups[1].Value, out var bitrate))
                    return bitrate;
            }
            return 0;
        }
    }
}
