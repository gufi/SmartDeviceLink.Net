using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDeviceLink.Net.Exceptions
{
    public class SessionUnregisteredException: Exception
    {
        public SessionUnregisteredException(string message):base(message)
        {
            
        }
    }
}
