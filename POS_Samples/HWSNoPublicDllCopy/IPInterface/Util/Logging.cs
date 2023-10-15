using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>
    /// Log Event object. Sent when a logging event occurs
    /// </summary>
    public class LogEventArgs
    {
        public LogEventArgs()
        {
        }

        public string Message { get; set; } = "";
        public LogLevel LogLevel { get; set; } = LogLevel.Off;
        public Exception Exception { get; set; } = null;
    }

    //
    // Summary:
    //     The 7 possible logging levels
    [Flags]
    public enum LogLevel
    {
        //
        // Summary:
        //     All logging levels
        All = 0,
        //
        // Summary:
        //     A trace logging level
        Trace = 1,
        //
        // Summary:
        //     A debug logging level
        Debug = 2,
        //
        // Summary:
        //     A info logging level
        Info = 4,
        //
        // Summary:
        //     A warn logging level
        Warn = 8,
        //
        // Summary:
        //     An error logging level
        Error = 16,
        //
        // Summary:
        //     A fatal logging level
        Fatal = 32,
        //
        // Summary:
        //     Do not log anything.
        Off = 64
    }
}
