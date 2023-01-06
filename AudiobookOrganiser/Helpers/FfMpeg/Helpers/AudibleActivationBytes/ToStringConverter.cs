using System.Reflection;

namespace FfMpeg.AudibleActivationBytesHelper.Diagn
{
    [Obfuscation(Exclude = true)]
    public abstract class ToStringConverter
    {
        public abstract string ToString(object o, string format = null);
    }
}
