using AudiobookOrganiser.Helpers.FfMpegWrapper.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.Extensions
{
    /// <summary>
    /// Used to get Aax (audible audio file) activation bytes so we can decode it
    /// </summary>
    public class AaxActivationClient
    {

        private readonly string _ffProbeExePath = string.Empty;

        /// <summary>
        /// Used to get Aax (audible audio file) activation bytes so we can decode it
        /// </summary>
        public AaxActivationClient(string ffProbExePath)
        {
            _ffProbeExePath = ffProbExePath;
        }

        public class AudibleKeyAndIv
        {
            public string Key { get; set; }
            public string Iv { get; set; }
        }


        /// <summary>
        /// Gets the checksum of aax file
        /// </summary>
        /// <param name="path">Input aax file path</param>
        /// <returns>Checksum</returns>
        public string GetActivationChecksum(string path)
        {
            try
            {
                using (var fs = File.OpenRead(path))
                using (var br = new BinaryReader(fs))
                {
                    fs.Position = 0x251 + 56 + 4;
                    byte[] checksum = br.ReadBytes(20);
                    return ToHexString(checksum);
                }
            }
            catch
            {
                return new FfProbe(_ffProbeExePath).GetChecksum(path);
            }
        }

        private string ToHexString(byte[] source)
        {
            return ToHexString(source, 0, source.Length);
        }

        private string ToHexString(byte[] source, int offset, int length)
        {
            var stringBuilder = new StringBuilder(length * 2);
            for (int i = offset; i < offset + length; i++)
            {
                stringBuilder.AppendFormat("{0:x2}", source[i]);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Get Aax activation key from aax file (uses all available methods to get it)
        /// </summary>
        /// <param name="path">Aax file path</param>
        /// <returns>Activation Key</returns>
        public string GetActivationBytes(string path, out string checksum)
        {
            checksum = string.Empty;
            string dChecksum = string.Empty;
            string activationBytes = string.Empty;

            try
            {
                checksum = GetActivationChecksum(path);
                dChecksum = checksum;
            }
            catch { }

            if (string.IsNullOrEmpty(activationBytes))
            {
                // Get from Audible Manger registry

                try
                {
                    activationBytes = Task.Run(() => ResolveActivationBytesFromAudibleApplicationsRegistry().Result).Result;
                }
                catch
                {
                    activationBytes = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(activationBytes))
            {
                // Get from audible-cli profiles

                try
                {
                    activationBytes = Task.Run(() => ResolveActivationBytesFromAudibleCliProfile().Result).Result;
                }
                catch
                {
                    activationBytes = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(activationBytes))
            {
                // Try use audible-cli directly, if it's installed and mappable

                try
                {
                    activationBytes = Task.Run(() => ResolveActivationBytesUsingAudibleCliDirectly().Result).Result;
                }
                catch
                {
                    activationBytes = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(activationBytes) &&
               !string.IsNullOrWhiteSpace(checksum) &&
               checksum != "0000000000000000000000000000000000000000")
            {
                // Get from rcrack

                try
                {
                    activationBytes = Task.Run(() => ResolveActivationBytesUsingRCrack(dChecksum).Result).Result;
                }
                catch
                {
                    activationBytes = string.Empty;
                }
            }

            return activationBytes;
        }

        /// <summary>
        /// Get Aax activation key from registry where Audible app, or Audible Manager is installed
        /// </summary>
        /// <returns>Activation Key</returns>
        public async Task<string> ResolveActivationBytesFromAudibleApplicationsRegistry()
        {
            try
            {
                return await Task.FromResult(new AudibleActivationBytesHelper.Lib.ActivationCode().ActivationCodes.ToList()[0]);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get Aax activation key using rcrack (won't work will hard antivirus)
        /// </summary>
        /// <param name="checksum">Aax file checksum</param>
        /// <returns>Activation Key</returns>
        public async Task<string> ResolveActivationBytesUsingRCrack(string checksum)
        {
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                string rcrackRootPath = Path.Combine(Path.GetDirectoryName(asm.Location), "Apps", "RCrack");
                string rcrackExePath = Path.Combine(Path.GetDirectoryName(asm.Location), "Apps", "RCrack", "win_rcrack.exe");

                string result = ProcessExtensions.ExecuteProcessAndReadStdOut(
                    rcrackExePath,
                    "\"" + rcrackRootPath + "\" -h " + checksum);

                if (!string.IsNullOrEmpty(result))
                {
                    int hexIndex = result.LastIndexOf("hex:");
                    if (hexIndex != -1)
                        return await Task.FromResult(result.Substring(hexIndex).Replace("hex:", ""));
                }
            }
            catch { }

            return string.Empty;
        }

        /// <summary>
        /// Get Aax activation key from audible-cli profile files
        /// </summary>
        /// <returns>Activation Key</returns>
        public async Task<string> ResolveActivationBytesFromAudibleCliProfile()
        {
            try
            {
                string audibleCliConfigPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Audible");

                if (!string.IsNullOrEmpty(audibleCliConfigPath))
                {
                    foreach (var file in Directory.GetFiles(audibleCliConfigPath, "*.*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            string activationBytes = JsonConvert.DeserializeObject<AudibleCliProfile>(File.ReadAllText(file)).activation_bytes;

                            if (!string.IsNullOrEmpty(activationBytes))
                                return await Task.FromResult(activationBytes);
                        }
                        catch { }
                    }
                }
            }
            catch { }

            return string.Empty;
        }

        /// <summary>
        /// Get Aax activation key from audible-cli profile files
        /// </summary>
        /// <returns>Activation Key</returns>
        public async Task<string> ResolveActivationBytesUsingAudibleCliDirectly()
        {
            try
            {
                string activationBytes = ProcessExtensions.ExecuteProcessAndReadStdOut("audible.exe", "activation-bytes");
                if (long.TryParse(activationBytes, out long result))
                    return await Task.FromResult(activationBytes.ToString());
            }
            catch { }

            return string.Empty;
        }

        /// <summary>
        /// Get Aaxc audible key and iv (value) from voucher file
        /// </summary>
        /// <param name="path">Aax file path</param>
        /// <returns>Activation Key</returns>
        public AudibleKeyAndIv GetAudibleKeyAndIv(string path, out string checksum)
        {
            checksum = string.Empty;

            try
            {
                string aaxChecksum = GetActivationChecksum(path);
                checksum = aaxChecksum;

                string voucherStr = File.ReadAllText(path);
                if (!string.IsNullOrEmpty(voucherStr))
                {
                    var voucherObj = JsonConvert.DeserializeObject<AudibleVoucher>(voucherStr);
                    string key = voucherObj.content_license.license_response.key;
                    string iv = voucherObj.content_license.license_response.iv;

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(iv))
                    {
                        return new AudibleKeyAndIv
                        {
                            Key = key,
                            Iv = iv
                        };
                    }
                }
            }
            catch { }

            return null;
        }

        public class AudibleVoucher
        {
            public Content_License content_license { get; set; }
            public string[] response_groups { get; set; }

            public class Content_License
            {
                public DateTime access_expiry_date { get; set; }
                public string acr { get; set; }
                public string[] allowed_users { get; set; }
                public string asin { get; set; }
                public Content_Metadata content_metadata { get; set; }
                public string drm_type { get; set; }
                public string license_id { get; set; }
                public License_Response license_response { get; set; }
                public string message { get; set; }
                public DateTime refresh_date { get; set; }
                public DateTime removal_date { get; set; }
                public string request_id { get; set; }
                public bool requires_ad_supported_playback { get; set; }
                public string status_code { get; set; }
                public string voucher_id { get; set; }
            }

            public class Content_Metadata
            {
                public Chapter_Info chapter_info { get; set; }
                public Content_Reference content_reference { get; set; }
                public Content_Url content_url { get; set; }
                public Last_Position_Heard last_position_heard { get; set; }
            }

            public class Chapter_Info
            {
                public int brandIntroDurationMs { get; set; }
                public int brandOutroDurationMs { get; set; }
                public Chapter[] chapters { get; set; }
                public bool is_accurate { get; set; }
                public int runtime_length_ms { get; set; }
                public int runtime_length_sec { get; set; }
            }

            public class Chapter
            {
                public int length_ms { get; set; }
                public int start_offset_ms { get; set; }
                public int start_offset_sec { get; set; }
                public string title { get; set; }
            }

            public class Content_Reference
            {
                public string acr { get; set; }
                public string asin { get; set; }
                public string content_format { get; set; }
                public int content_size_in_bytes { get; set; }
                public string file_version { get; set; }
                public string marketplace { get; set; }
                public string sku { get; set; }
                public string tempo { get; set; }
                public string version { get; set; }
            }

            public class Content_Url
            {
                public string offline_url { get; set; }
            }

            public class Last_Position_Heard
            {
                public string last_updated { get; set; }
                public int position_ms { get; set; }
                public string status { get; set; }
            }

            public class License_Response
            {
                public string key { get; set; }
                public string iv { get; set; }
                public DateTime refreshDate { get; set; }
                public DateTime removalOnExpirationDate { get; set; }
                public Rule[] rules { get; set; }
            }

            public class Rule
            {
                public Parameter[] parameters { get; set; }
                public string name { get; set; }
            }

            public class Parameter
            {
                public DateTime expireDate { get; set; }
                public string type { get; set; }
                public string[] directedIds { get; set; }
            }
        }
    }
}