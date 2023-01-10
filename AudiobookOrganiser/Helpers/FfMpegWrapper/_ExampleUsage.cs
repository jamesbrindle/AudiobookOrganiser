using AudiobookOrganiser.Helpers.FfMpegWrapper.Enums;
using AudiobookOrganiser.Helpers.FfMpegWrapper.Events;
using System;
using System.Threading.Tasks;

namespace AudiobookOrganiser.Helpers.FfMpegWrapper
{
    // Somme example usage methods using FFMpeg.Net - View in JBToolkit Repo
    public class ExampleUsage
    {
        public void GrabThumbnailFromVideo()
        {
            var inputFile = new MediaFile(@"C:\Path\To_Video.flv");
            var outputFile = new MediaFile(@"C:\Path\To_Save_Image.jpg");

            var ffmpeg = new FfMpeg("C:\\ffmpeg\\ffmpeg.exe");
            // Saves the frame located on the 15th second of the video.
            var options = new GetThumbnailOptions { Seek = TimeSpan.FromSeconds(15) };

            // Async
            // await ffmpeg.GetThumbnailAsync(inputFile, outputFile, options);
            ffmpeg.GetThumbnail(inputFile, outputFile, options);
        }

        public void RetrieveMetaData()
        {
            var inputFile = new MediaFile(@"C:\Path\To_Video.avi");

            var ffmpeg = new FfMpeg("C:\\ffmpeg\\ffmpeg.exe");

            // Async
            //var metadata = await ffmpeg.GetMetadataAsync(inputFile);
            var metadata = ffmpeg.GetMetaData(inputFile);

            Console.WriteLine(metadata.Duration);
        }

        public void BasicConversion()
        {
            var inputFile = new MediaFile(@"C:\Path\To_Video.avi");
            var outputFile = new MediaFile(@"C:\Path\To_Save_New_Video.mp4");

            var ffmpeg = new FfMpeg("C:\\ffmpeg\\ffmpeg.exe");

            var options = new ConversionOptions
            {
                Format = Format.mp4,
                Codec = Codec.libx264
            };

            // Async
            //await ffmpeg.ConvertAsync(inputFile, outputFile, options);
            ffmpeg.Convert(inputFile, outputFile, options);
        }

        public void ConvertFlashVideoToDVD()
        {
            var inputFile = new MediaFile(@"C:\Path\To_Video.avi");
            var outputFile = new MediaFile(@"C:\Path\To_Save_New_DVD.mkv");

            var conversionOptions = new ConversionOptions
            {
                Target = Target.DVD,
                TargetStandard = TargetStandard.PAL
            };

            var ffmpeg = new FfMpeg("C:\\ffmpeg\\ffmpeg.exe");

            // Async
            //await ffmpeg.ConvertAsync(inputFile, outputFile, conversionOptions);
            ffmpeg.Convert(inputFile, outputFile, conversionOptions);
        }

        public void CutVideoDownToSmallerLength()
        {
            var inputFile = new MediaFile(@"C:\Path\To_Video.avi");
            var outputFile = new MediaFile(@"C:\Path\To_Save_ExtractedVideo.avi");

            var ffmpeg = new FfMpeg("C:\\ffmpeg\\ffmpeg.exe");
            var options = new ConversionOptions();

            // This example will create a 25 second video, starting from the 
            // 30th second of the original video.
            //// First parameter requests the starting frame to cut the media from.
            //// Second parameter requests how long to cut the video.
            options.CutMedia(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(25));

            // Async
            // await ffmpeg.ConvertAsync(inputFile, outputFile, options);
            ffmpeg.Convert(inputFile, outputFile, options);
        }

        // SUBSCRIBE TO EVENTS

        public async Task StartConverting()
        {
            var inputFile = new MediaFile(@"C:\Path\To_Video.avi");
            var outputFile = new MediaFile(@"C:\Path\To_Save_New_Video.mp4");

            var ffmpeg = new FfMpeg("C:\\ffmpeg\\ffmpeg.exe");
            ffmpeg.Progress += OnProgress;
            ffmpeg.Data += OnData;
            ffmpeg.Error += OnError;
            ffmpeg.Complete += OnComplete;
            await ffmpeg.ConvertAsync(inputFile, outputFile);
        }

        private void OnProgress(object sender, ConversionProgressEventArgs e)
        {
            Console.WriteLine("[{0} => {1}]", e.Input.FileInfo.Name, e.Output.FileInfo.Name);
            Console.WriteLine("Bitrate: {0}", e.Bitrate);
            Console.WriteLine("Fps: {0}", e.Fps);
            Console.WriteLine("Frame: {0}", e.Frame);
            Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
            Console.WriteLine("Size: {0} kb", e.SizeKb);
            Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
        }

        private void OnData(object sender, ConversionDataEventArgs e)
        {
            Console.WriteLine("[{0} => {1}]: {2}", e.Input.FileInfo.Name, e.Output.FileInfo.Name, e.Data);
        }

        private void OnComplete(object sender, ConversionCompleteEventArgs e)
        {
            Console.WriteLine("Completed conversion from {0} to {1}", e.Input.FileInfo.FullName,
                e.Output.FileInfo.FullName);
        }

        private void OnError(object sender, ConversionErrorEventArgs e)
        {
            Console.WriteLine("[{0} => {1}]: Error: {2}\n{3}", e.Input.FileInfo.Name, e.Output.FileInfo.Name,
                e.Exception.ExitCode, e.Exception.InnerException);
        }
    }
}