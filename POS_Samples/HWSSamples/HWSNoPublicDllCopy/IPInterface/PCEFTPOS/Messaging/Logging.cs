using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>
    /// Log Event object. Sent when a logging event occurs
    /// </summary>
    //public class LogEventArgs
    //{
    //    public LogEventArgs()
    //    {
    //    }

    //    public string Message { get; set; } = "";
    //    public LogLevel LogLevel { get; set; } = LogLevel.Off;
    //    public Exception Exception { get; set; } = null;
    //}

    //
    // Summary:
    //     The 7 possible logging levels
    //[Flags]
    //public enum LogLevel
    //{
    //    //
    //    // Summary:
    //    //     All logging levels
    //    All = 0,
    //    //
    //    // Summary:
    //    //     A trace logging level
    //    Trace = 1,
    //    //
    //    // Summary:
    //    //     A debug logging level
    //    Debug = 2,
    //    //
    //    // Summary:
    //    //     A info logging level
    //    Info = 4,
    //    //
    //    // Summary:
    //    //     A warn logging level
    //    Warn = 8,
    //    //
    //    // Summary:
    //    //     An error logging level
    //    Error = 16,
    //    //
    //    // Summary:
    //    //     A fatal logging level
    //    Fatal = 32,
    //    //
    //    // Summary:
    //    //     Do not log anything.
    //    Off = 64
    //}

    public class TraceRecord
    {
        public TraceRecord()
        {
            Message = "";
            Data = null;
            Level = LogLevel.Off;
        }

        public void Set(string message)
        {
            Message = message;
        }

        public void Set(string message, object data)
        {
            Message = message;
            Data = data;
        }

        public void Set(string message, object data, Exception exception)
        {
            Message = message;
            Data = data;
            Exception = exception;
        }

        public void Set(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }

        /// <summary>
        /// OFF – The OFF Level has the highest possible rank and is intended to turn off logging.
        /// FATAL – The FATAL level designates very severe error events that will presumably lead the application to abort. 
        /// ERROR – The ERROR level designates error events that might still allow the application to continue running.
        /// WARN – The WARN level designates potentially harmful situations.
        /// INFO – The INFO level designates informational messages that highlight the progress of the application at coarse-grained level.
        /// DEBUG – The DEBUG Level designates fine-grained informational events that are most useful to debug an application.
        /// TRACE – The TRACE Level designates finer-grained informational events than the DEBUG
        /// ALL -The ALL Level has the lowest possible rank and is intended to turn on all logging. 
        /// </summary>
        public LogLevel Level { get; set; }

        public string Message { get; set; }
        public object Data { get; set; }
        public Exception Exception { get; set; }
    }
}
