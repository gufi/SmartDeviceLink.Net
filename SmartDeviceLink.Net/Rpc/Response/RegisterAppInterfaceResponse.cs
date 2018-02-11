using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Rpc.Base;
using SmartDeviceLink.Net.Rpc.BasicCommunication;

namespace SmartDeviceLink.Net.Rpc.Response
{
    public class RegisterAppInterfaceResponse 
    {
        public List<AudioPassThruCapability> AudioPassThruCapabilities { get; set; }
        public List<ButtonCapability> ButtonCapabilities { get; set; }
        public DisplayCapabilities DisplayCapabilities { get; set; }
        public HmiCapabilities HmiCapabilities { get; set; }
        public string HmiDisplayLanguage { get; set; }
        public List<string> HmiZoneCapabilities { get; set; }
        public string Info { get; set; }
        public int Language { get; set; }
        public PcmStreamCapabilities PcmStreamCapabilities { get; set; }
        public PresetBankCapabilities PresetBankCapabilities { get; set; }
        public string ResultCode { get; set; }
        public string SdlVersion { get; set; }
        public List<SoftButtonCapability> SoftButtonCapabilities { get; set; }
        public List<string> SpeechCapabilities { get; set; }
        public bool Success { get; set; }
        public List<int> SupportedDiagModes { get; set; }
        public SyncMsgVersion SyncMsgVersion { get; set; }
        public string SystemSoftwareVersion { get; set; }
        public VehicleType VehicleType { get; set; }
        public List<string> VrCapabilities { get; set; }
    }
}
