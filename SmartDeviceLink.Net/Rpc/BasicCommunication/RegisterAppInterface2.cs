using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Rpc.Base;

namespace SmartDeviceLink.Net.Rpc.BasicCommunication
{

    //public class RegisterAppInterface : RpcRequest<RegisterAppInterfaceParameters>
    //{
    //    public override FunctionID FunctionId { get; set; } = FunctionID.RegisterAppInterface;
    //}

    public class RegisterAppInterface : RpcRequest
    {
        public override FunctionID FunctionId { get; set; } = FunctionID.RegisterAppInterface;
        public List<Ttsname> ttsName { get; set; } 
        public string hmiDisplayLanguageDesired { get; set; } = "EN-US";
        //public List<string> appHMIType { get; set; } = new List<string>(){"SOCIAL","MEDIA"};
        public string appID { get; set; } = "12312312";
        public string languageDesired { get; set; } = "EN-US";
        public Deviceinfo deviceInfo { get; set; }= new Deviceinfo();
        public string appName { get; set; } = "Default";
        public string ngnMediaScreenAppName { get; set; } = "Df";
        public bool isMediaApplication { get; set; } = true;
        public List<string> vrSynonyms { get; set; }= new List<string> {"Default"};
        public Syncmsgversion syncMsgVersion { get; set; } = new Syncmsgversion();
        //public string hashID { get; set; } = "0000000";
    }

    public class Deviceinfo
    {
        public string hardware { get; set; } = "My Pc";
        //public string firmwareRev { get; set; } = "1.0.0";
        public string os { get; set; } = "Windows";
        public string osVersion { get; set; } = "10";
       // public string carrier { get; set; } = "Nobody";
       // public int maxNumberRFCOMMPorts { get; set; } = 0;
    }

    public class Syncmsgversion
    {
        public int majorVersion { get; set; } = 4;
        public int minorVersion { get; set; } = 0;
       // public int patchVersion { get; set; } = 0;
    }

    public class Ttsname
    {
        public string text { get; set; }
        public string type { get; set; }
    }

}
