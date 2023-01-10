using System.Diagnostics;
using System.Reflection;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.AudibleActivationBytesHelper
{
    [Obfuscation(Exclude = true)]
    public interface IProcessList
    {
        bool Add(Process process);
        bool Remove(Process process);
    }
}
