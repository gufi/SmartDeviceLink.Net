using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartDeviceLink.Net.Transport.Interfaces
{
    public interface ITransport : IDisposable
    {
        Task SendAsync(OutgoingTransportPacket packet);
    }
}
