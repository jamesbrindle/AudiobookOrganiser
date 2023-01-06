using System.Reflection;

namespace FfMpeg.AudibleActivationBytesHelper
{
    [Obfuscation(Exclude = true)]
    public interface IInteractionCallback<T, out TResult>
    {
        TResult Interact(T value);
    }
}
