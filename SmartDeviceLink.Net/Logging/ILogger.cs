using System;

namespace SmartDeviceLink.Net.Logging
{
    public interface ILogger
    {
        LogLevel LogLevel { get; set; }
        void LogFatal(string message, object obj = null, Exception e = null);
        void LogError(string message, object obj = null, Exception e = null);
        void LogVerbose(string message, object obj = null);
        void LogDebug(string message, object obj = null);
        void LogInfo(string message);
        void LogWarning(string message);
    }
}