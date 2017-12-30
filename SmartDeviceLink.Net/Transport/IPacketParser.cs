namespace SmartDeviceLink.Net.Transport
{
    public interface IPacketParser
    {
        void HandleByte(byte data);
    }
}