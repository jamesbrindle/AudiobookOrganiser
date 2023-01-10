using System.Reflection;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.AudibleActivationBytesHelper
{
    [Obfuscation(Exclude = true)]
    public enum ECallbackType { info, infoCancel, warning, error, errorQuestion, errorQuestion3, question, question3, custom }

    [Obfuscation(Exclude = true)]
    public class InteractionMessage
    {
        public ECallbackType Type;
        public string Message;
    }

    [Obfuscation(Exclude = true)]
    public class InteractionMessage<T> : InteractionMessage where T : struct, System.Enum
    {
        public T Custom;
    }

}
