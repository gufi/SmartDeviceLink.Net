using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Protocol;

namespace SmartDeviceLink.Net.Transport.Interfaces
{
    public interface ITransport : IDisposable
    {
        Task SendAsync(TransportPacket packet);
        Action<TransportPacket> OnRecievedPacket { get; set; }
    }
}
