using System;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper.Events
{
    public class ConversionDataEventArgs : EventArgs
    {
        public ConversionDataEventArgs(string data, MediaFile input, MediaFile output)
        {
            Data = data;
            Input = input;
            Output = output;
        }

        public string Data { get; }
        public MediaFile Input { get; }
        public MediaFile Output { get; }
    }
}