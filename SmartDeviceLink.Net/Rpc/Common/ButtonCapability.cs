namespace SmartDeviceLink.Net.Rpc.Response
{
    public class ButtonCapability
    {
        public bool LongPressAvailable { get; set; }
        public string Name { get; set; }
        public bool ShortPressAvailable { get; set; }
        public bool UpDownAvailable { get; set; }
    }
}