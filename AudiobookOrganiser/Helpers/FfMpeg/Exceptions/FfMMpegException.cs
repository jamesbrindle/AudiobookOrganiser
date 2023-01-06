using System;

namespace FfMpeg.Exceptions
{
    public class FfMMpegException : Exception
    {
        public FfMMpegException(int exitCode)
        {
            ExitCode = exitCode;
        }

        public FfMMpegException(string message, int exitCode) : base(message)
        {
            ExitCode = exitCode;
        }

        public FfMMpegException(string message, Exception innerException, int exitCode) : base(message, innerException)
        {
            ExitCode = exitCode;
        }

        public int ExitCode { get; }
    }
}