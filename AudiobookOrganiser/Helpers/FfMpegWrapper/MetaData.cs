using System;
using System.Collections.Generic;
using System.IO;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    /// <summary>
    ///     Media (audio or video) meta data object - Holds read media meta data.
    /// </summary>
    public class MetaData
    {
        public MetaData()
        { }

        public TimeSpan Duration { get; internal set; }
        public Video VideoData { get; internal set; }
        public Audio AudioData { get; internal set; }
        public FileInfo FileInfo { get; internal set; }
        public Dictionary<string, string> Tags { get; internal set; }

        public override string ToString()
        {
            return $"Duration: {Duration}\nVideo MetaData:\n{VideoData}\nAudio MetaData:\n{AudioData}";
        }

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
            {
                return
                    $"Format: {Format}\nColorModel: {ColorModel}\nFrameSize: {FrameSize}\nBitRateKbs: {BitRateKbs}\nFps: {Fps}";
            }
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
            {
                return
                    $"Format: {Format}\nSampleRate: {SampleRate}\nChannelOuput: {ChannelOutput}\nBitRateKbs: {BitRateKbs}";
            }
        }
    }
}