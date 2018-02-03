using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Rpc.Base;
using SmartDeviceLink.Net.Rpc.BasicCommunication;

namespace SmartDeviceLink.Net.Rpc.Response
{
    public class RegisterAppInterfaceResponse 
    {
        public List<Audiopassthrucapability> audioPassThruCapabilities { get; set; }
        public List<Buttoncapability> buttonCapabilities { get; set; }
        public Displaycapabilities displayCapabilities { get; set; }
        public Hmicapabilities hmiCapabilities { get; set; }
        public string hmiDisplayLanguage { get; set; }
        public List<string> hmiZoneCapabilities { get; set; }
        public string info { get; set; }
        public int language { get; set; }
        public Pcmstreamcapabilities pcmStreamCapabilities { get; set; }
        public Presetbankcapabilities presetBankCapabilities { get; set; }
        public string resultCode { get; set; }
        public string sdlVersion { get; set; }
        public List<Softbuttoncapability> softButtonCapabilities { get; set; }
        public List<string> speechCapabilities { get; set; }
        public bool success { get; set; }
        public List<int> supportedDiagModes { get; set; }
        public Syncmsgversion syncMsgVersion { get; set; }
        public string systemSoftwareVersion { get; set; }
        public Vehicletype vehicleType { get; set; }
        public List<string> vrCapabilities { get; set; }
    }

    public class Displaycapabilities
    {
        public string displayType { get; set; }
        public bool graphicSupported { get; set; }
        public List<Imagefield> imageFields { get; set; }
        public List<string> mediaClockFormats { get; set; }
        public int numCustomPresetsAvailable { get; set; }
        public Screenparams screenParams { get; set; }
        public List<string> templatesAvailable { get; set; }
        public Textfield[] textFields { get; set; }
    }

    public class Screenparams
    {
        public Resolution resolution { get; set; }
        public Toucheventavailable touchEventAvailable { get; set; }
    }

    public class Resolution
    {
        public int resolutionHeight { get; set; }
        public int resolutionWidth { get; set; }
    }

    public class Toucheventavailable
    {
        public bool doublePressAvailable { get; set; }
        public bool multiTouchAvailable { get; set; }
        public bool pressAvailable { get; set; }
    }

    public class Imagefield
    {
        public Imageresolution imageResolution { get; set; }
        public List<string> imageTypeSupported { get; set; }
        public string name { get; set; }
    }

    public class Imageresolution
    {
        public int resolutionHeight { get; set; }
        public int resolutionWidth { get; set; }
    }

    public class Textfield
    {
        public string characterSet { get; set; }
        public object name { get; set; }
        public int rows { get; set; }
        public int width { get; set; }
    }

    public class Hmicapabilities
    {
        public bool navigation { get; set; }
        public bool phoneCall { get; set; }
        public bool remoteControl { get; set; }
        public bool videoStreaming { get; set; }
    }

    public class Pcmstreamcapabilities
    {
        public string audioType { get; set; }
        public string bitsPerSample { get; set; }
        public string samplingRate { get; set; }
    }

    public class Presetbankcapabilities
    {
        public bool onScreenPresetsAvailable { get; set; }
    }

    public class Vehicletype
    {
        public string make { get; set; }
        public string model { get; set; }
        public string modelYear { get; set; }
        public string trim { get; set; }
    }

    public class Audiopassthrucapability
    {
        public string audioType { get; set; }
        public string bitsPerSample { get; set; }
        public string samplingRate { get; set; }
    }

    public class Buttoncapability
    {
        public bool longPressAvailable { get; set; }
        public string name { get; set; }
        public bool shortPressAvailable { get; set; }
        public bool upDownAvailable { get; set; }
    }

    public class Softbuttoncapability
    {
        public bool imageSupported { get; set; }
        public bool longPressAvailable { get; set; }
        public bool shortPressAvailable { get; set; }
        public bool upDownAvailable { get; set; }
    }

}
