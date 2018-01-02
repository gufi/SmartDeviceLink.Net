using System;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Converters;
using SmartDeviceLink.Net.Exceptions;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol;
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
        private ILogger _logger => Logger.SdlLogger;
        private readonly ITransport _transport;
        private readonly WiProProtocol _protocol;
        private object _session = null;
        private bool isDisposed;

        public SdlClient(ITransport transport)
        {
            _transport = transport;
            _protocol = new WiProProtocol();
        }

        public void StartSession()
        {
            _session = new object();
        }

        public async Task SendAsync(RpcRequest request)
        {
            if(_session == null)
            { throw new SessionException("Session does not exist, StartSession() must be called prior to sending RpcRequests");}
            var protocolMessage = request.ToProtocolMessage();
            _logger.LogVerbose("Created Protocol Message",protocolMessage);
            _logger.LogVerbose("Bytes: " + protocolMessage.JsonSize);
            var transportPackets = _protocol.CreateTransportPackets(protocolMessage);
            foreach (var pack in transportPackets)
            {
                await _transport.SendAsync(pack);
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
                _transport.Dispose();
            isDisposed = true;
        }
    }
}