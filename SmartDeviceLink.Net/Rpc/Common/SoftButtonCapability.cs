namespace SmartDeviceLink.Net.Rpc.Response
{
    public class SoftButtonCapability
    {
        public bool ImageSupported { get; set; }
        public bool LongPressAvailable { get; set; }
        public bool ShortPressAvailable { get; set; }
        public bool UpDownAvailable { get; set; }
    }
}