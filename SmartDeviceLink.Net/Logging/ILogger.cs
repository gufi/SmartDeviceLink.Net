using System;

namespace SmartDeviceLink.Net.Logging
{
    public interface ILogger
    {
        void LogError(string message, object obj = null, Exception e = null);
        void LogVerbose(string message, object obj = null);
    }
}