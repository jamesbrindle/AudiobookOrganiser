using System;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.Events
{
    public class ConversionCompleteEventArgs : EventArgs
    {
        public ConversionCompleteEventArgs(MediaFile input, MediaFile output)
        {
            Input = input;
            Output = output;
        }

        public MediaFile Input { get; }
        public MediaFile Output { get; }
    }
}