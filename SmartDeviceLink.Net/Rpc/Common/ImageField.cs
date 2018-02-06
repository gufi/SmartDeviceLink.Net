using System.Collections.Generic;

namespace SmartDeviceLink.Net.Rpc.Response
{
    public class ImageField
    {
        public ImageResolution ImageResolution { get; set; }
        public List<string> ImageTypeSupported { get; set; }
        public string Name { get; set; }
    }
}