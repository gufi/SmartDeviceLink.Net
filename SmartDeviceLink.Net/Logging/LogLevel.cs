using System;

namespace SmartDeviceLink.Net.Logging
{
    public enum LogLevel 
    {
        Verbose =  0b111111,
        Debug =    0b011111,
        Info =     0b001111,
        Warning =  0b000111,
        Error =    0b000011,
        Fatal =    0b000001,
    }
}