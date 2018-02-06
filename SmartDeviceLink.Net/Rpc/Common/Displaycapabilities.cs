using System.Collections.Generic;

namespace SmartDeviceLink.Net.Rpc.Response
{
    public class DisplayCapabilities
    {
        public string DisplayType { get; set; }
        public bool GraphicSupported { get; set; }
        public List<ImageField> ImageFields { get; set; }
        public List<string> MediaClockFormats { get; set; }
        public int NumCustomPresetsAvailable { get; set; }
        public ScreenParams ScreenParams { get; set; }
        public List<string> TemplatesAvailable { get; set; }
        public TextField[] TextFields { get; set; }
    }
}