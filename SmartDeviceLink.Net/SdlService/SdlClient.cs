using System;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Converters;
using SmartDeviceLink.Net.Rpc.Base;
using SmartDeviceLink.Net.Transport;
using SmartDeviceLink.Net.Transport.Interfaces;

namespace SmartDeviceLink.Net.SdlService
{
    /// <summary>
    ///     Based on sdl_Android Sdl Proxy Base
    /// </summary>
    public class SdlClient : IDisposable
    {
        private readonly ITransport _transport;
        private bool isDisposed;

        public SdlClient(ITransport transport)
        {
            _transport = transport;
        }

        public void Dispose()
        {
            if (!isDisposed)
                _transport.Dispose();
            isDisposed = true;
        }

        public Task SendAsync(RpcRequest request)
        {
            var protocolMessage = request.ToProtocolMessage();
            protocolMessage.SessionID = 1; // This is a stub, this should be found from the eventual implementation of the session object
            // Create Protocol Message
            // line 1737 from SdlProxyBase
            // WiProProtocol -> line 168
            // then convert to OutgoingTransportPacket
            var transportPacket = protocolMessage.ToOutgoingTransportPacket();
            // now convert to byytes WiProProtocol -> line 168-199
            var bytes = transportPacket.ToBytes();



            return _transport.SendAsync(transportPacket);
        }
    }
}