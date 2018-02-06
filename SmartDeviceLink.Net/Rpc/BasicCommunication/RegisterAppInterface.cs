using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Rpc.Base;

namespace SmartDeviceLink.Net.Rpc.BasicCommunication
{


    public class RegisterAppInterface : RpcRequest
    {
        public override FunctionID FunctionId { get; set; } = FunctionID.RegisterAppInterface;
        public List<Ttsname> TtsName { get; set; } 
        public string HmiDisplayLanguageDesired { get; set; } = "EN-US";
        //public List<string> appHMIType { get; set; } = new List<string>(){"SOCIAL","MEDIA"};
        // ReSharper disable once InconsistentNaming
        public string AppID { get; set; } = "12312312";
        public string LanguageDesired { get; set; } = "EN-US";
        public Deviceinfo DeviceInfo { get; set; }= new Deviceinfo();
        public string AppName { get; set; } = "Default";
        public string NgnMediaScreenAppName { get; set; } = "Df";
        public bool IsMediaApplication { get; set; } = true;
        public List<string> VrSynonyms { get; set; }= new List<string> {"Default"};
        public Syncmsgversion SyncMsgVersion { get; set; } = new Syncmsgversion();
        //public string hashID { get; set; } = "0000000";
    }
}
