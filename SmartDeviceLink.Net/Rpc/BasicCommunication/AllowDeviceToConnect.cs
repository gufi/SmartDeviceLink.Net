using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDeviceLink.Net.Rpc.BasicCommunication
{
    public class AllowDeviceToConnect
    {
        public AllowDeviceToConnect()
        {
            DeviceInfo = new DeviceInfo();
        }
        public DeviceInfo DeviceInfo { get; set; }
    }

    public class DeviceInfo
    {
        public String Name { get; set; }
        public int Id { get; set; }
    }
}
