using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Common;
using SmartDeviceLink.Net.Converters;
using SmartDeviceLink.Net.Exceptions;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Protocol.Session;
using SmartDeviceLink.Net.Rpc.Base;
using SmartDeviceLink.Net.Transport;
using SmartDeviceLink.Net.Transport.Enums;
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
        private readonly WiProProtocolManager _protocol;
        

        public SdlClient(ITransport transport)
        {
            _protocol = new WiProProtocolManager(transport);
        }

       public async Task<RpcRequest> SendAsync(RpcRequest request)
        {
            var protocolMessage = request.ToProtocolMessage();
            var result = await _protocol.SendAsync(protocolMessage).TimeoutAfter(10000);
            return  ToRpcRequest(result);
        }

        public async Task StartSession()
        {
            await _protocol.StartSessionAsync(ServiceType.Rpc);
        }

        private RpcRequest ToRpcRequest(ProtocolMessage message)
        {
            if (message == null) return null;
            _logger.LogVerbose(message.MessageType.ToString(),message);
            if(message.JsonSize > 0)
                _logger.LogVerbose("Json",Encoding.ASCII.GetString(message.Payload));
            return null;
        }

        public void Dispose()
        {
            _transport?.Dispose();
            _protocol?.Dispose();
        }
    }
}