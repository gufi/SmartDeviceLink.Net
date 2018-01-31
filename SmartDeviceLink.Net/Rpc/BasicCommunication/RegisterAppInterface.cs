using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Rpc.Base;

namespace SmartDeviceLink.Net.Rpc.BasicCommunication
{
    public class RegisterAppInterface: RpcRequest
    {
        public string SyncMsgVersion { get; set; } = "4.0.0";
        public string TtsName { get; set; }
        public string HmiDisplayLanguageDesired { get; set; } = "en-US";
        public string AppHMIType { get; set; }
        public string AppId { get; set; } = "000000001";
        public string AppName { get; set; } = "Default";
        public string LanguageDesired { get; set; } = "en-US";
        public string NgnMediaScreenAppName { get; set; } = "Default";
        public bool IsMediaApplication { get; set; } = false;
        public string VrSynonyms { get; set; }
        public string HashID { get; set; }
    }
}
