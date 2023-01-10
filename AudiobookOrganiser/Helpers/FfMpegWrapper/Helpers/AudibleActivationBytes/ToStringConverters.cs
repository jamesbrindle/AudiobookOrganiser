using AudiobookOrganiser.Helpers.FfMpegWrapper.AudibleActivationBytesHelper.Diagn;
using AudiobookOrganiser.Helpers.FfMpegWrapper.AudibleActivationBytesHelper.Ex;
using System;
using System.Reflection;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.AudibleActivationBytesHelper.Lib
{
    [Obfuscation(Exclude = true)]
    class ToStringConverterActivationCode : ToStringConverter
    {
        public override string ToString(object o, string format = null)
        {
            try
            {
                uint? ac = (uint?)o;
                return ac.HasValue ? "XXXXXXXX" : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    [Obfuscation(Exclude = true)]
    class ToStringConverterPath : ToStringConverter
    {
        public override string ToString(object o, string format = null)
        {
            if (o is string s)
                return s.SubstitUser();
            else
                return null;
        }
    }
}
