using FfMpeg.Exceptions;
using System;

namespace FfMpeg.Events
{
    public class ConversionErrorEventArgs : EventArgs
    {
        public ConversionErrorEventArgs(FfMMpegException exception, MediaFile input, MediaFile output)
        {
            Exception = exception;
            Input = input;
            Output = output;
        }

        public FfMMpegException Exception { get; }
        public MediaFile Input { get; }
        public MediaFile Output { get; }
    }
}