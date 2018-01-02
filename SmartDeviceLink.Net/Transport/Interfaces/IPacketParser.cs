namespace SmartDeviceLink.Net.Transport.Interfaces
{
    public interface IPacketParser
    {
        void HandleByte(byte data);
    }
}