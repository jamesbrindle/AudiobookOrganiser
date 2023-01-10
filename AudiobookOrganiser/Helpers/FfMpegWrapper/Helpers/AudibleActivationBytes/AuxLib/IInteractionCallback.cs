using System.Reflection;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.AudibleActivationBytesHelper
{
    [Obfuscation(Exclude = true)]
    public interface IInteractionCallback<T, out TResult>
    {
        TResult Interact(T value);
    }
}
