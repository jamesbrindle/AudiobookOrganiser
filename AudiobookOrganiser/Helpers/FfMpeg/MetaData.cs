using System;
using System.Collections.Generic;
using System.IO;

namespace FfMpeg
{
    public class MetaData
    {
        internal MetaData()
        { }

        public TimeSpan Duration { get; internal set; }
        public Video VideoData { get; internal set; }
        public Audio AudioData { get; internal set; }
        public FileInfo FileInfo { get; internal set; }
        public Dictionary<string, string> Tags { get; internal set; }

        public override string ToString()
            => $"Duration: {Duration}\nVideo MetaData:\n{VideoData}\nAudio MetaData:\n{AudioData}";

        public class Video
        {
            internal Video()
            { }

            public string Format { get; internal set; }
            public string ColorModel { get; internal set; }
            public string FrameSize { get; internal set; }
            public int? BitRateKbs { get; internal set; }
            public double Fps { get; internal set; }

            public override string ToString()
                => $"Format: {Format}\nColorModel: {ColorModel}\nFrameSize: {FrameSize}\nBitRateKbs: {BitRateKbs}\nFps: {Fps}";
        }

        public class Audio
        {
            internal Audio()
            { }

            public string Format { get; internal set; }
            public string SampleRate { get; internal set; }
            public string ChannelOutput { get; internal set; }
            public int BitRateKbs { get; internal set; }

            public override string ToString()
                => $"Format: {Format}\nSampleRate: {SampleRate}\nChannelOuput: {ChannelOutput}\nBitRateKbs: {BitRateKbs}";
        }
    }
}