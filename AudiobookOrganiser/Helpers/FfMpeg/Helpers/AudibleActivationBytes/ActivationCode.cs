using FfMpeg.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FfMpeg.AudibleActivationBytesHelper.Lib
{
    [Obfuscation(Exclude = true)]
    interface IActivationCode
    {
        IEnumerable<string> ActivationCodes { get; }
        bool HasActivationCode { get; }
    }

    [Obfuscation(Exclude = true)]
    class ActivationCode : IActivationCode
    {

        private List<string> _activationCodes = new List<string>();

        public IEnumerable<uint> NumericCodes => _activationCodes?.Select(s => Convert.ToUInt32(s, 16)).ToList();
        public IEnumerable<string> ActivationCodes => _activationCodes;
        public bool HasActivationCode => ActivationCodes?.Count() > 0;

        public ActivationCode()
        {
            init();
        }

        private void init()
        {
            _activationCodes.Clear();

            // Audible-cli

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
                            string activationBytes = Newtonsoft.Json.JsonConvert.DeserializeObject<AudibleCliProfile>(File.ReadAllText(file)).activation_bytes;

                            if (!string.IsNullOrEmpty(activationBytes))
                            {
                                if (!_activationCodes.Contains(activationBytes))
                                    _activationCodes.Add(activationBytes);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Audible app or Audible Manager

            ActivationCodeRegistry registryCodes = new ActivationCodeRegistry();
            if (registryCodes.HasActivationCode)
            {
                _activationCodes = _activationCodes.Union(registryCodes.ActivationCodes).ToList();
            }

            ActivationCodeApp appCodes = new ActivationCodeApp();
            if (appCodes.HasActivationCode)
            {
                _activationCodes = _activationCodes.Union(appCodes.ActivationCodes).ToList();
            }
        }

        public bool ReinitActivationCode()
        {
            init();
            return HasActivationCode;
        }
    }

    namespace Ex
    {
        [Obfuscation(Exclude = true)]
        public static class ActivationCodeEx
        {
            public static string ToHexString(this uint code)
            {
                return code.ToString("X8");
            }

            public static string ToHexDashString(this uint? code)
            {
                if (code.HasValue)
                    return code.Value.ToHexDashString();
                else
                    return string.Empty;
            }

            public static string ToHexDashString(this uint code)
            {
                var bytes = BitConverter.GetBytes(code);
                Array.Reverse(bytes);
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    if (sb.Length > 0)
                        sb.Append('-');
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }

            public static uint ToUInt32(this IEnumerable<string> chars)
            {
                if (chars.Count() == 4)
                {
                    var sb = new StringBuilder();
                    foreach (string s in chars)
                        sb.Append(s);
                    try
                    {
                        return Convert.ToUInt32(sb.ToString(), 16);
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
                else
                    return 0;
            }

        }
    }
}
