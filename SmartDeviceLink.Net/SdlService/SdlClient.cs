using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartDeviceLink.Net.Common;
using SmartDeviceLink.Net.Converters;
using SmartDeviceLink.Net.Exceptions;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Protocol.Models;
using SmartDeviceLink.Net.Protocol.Session;
using SmartDeviceLink.Net.Rpc.Base;
using SmartDeviceLink.Net.Rpc.BasicCommunication;
using SmartDeviceLink.Net.Rpc.Response;
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
        public RegisterAppInterfaceResponse HmiInfo { get; private set; }
        public int RequestTimeout { get; set; }
        public SdlClient(ITransport transport,int timeoutMs = 10000)
        {
            _protocol = new WiProProtocolManager(transport);
            RequestTimeout = timeoutMs;
        }


        public async Task<T> SendAsync<T>(RpcRequest request) 
        {
            var protocolMessage = request.ToProtocolMessage();
            var result = await _protocol.SendAsync(protocolMessage).TimeoutAfter(RequestTimeout);
            var requestResponse = ToRpcRequest<T>(result);
            return requestResponse;
        }

        public async Task RegisterAppWithHmi(RegisterAppInterface raiRequest)
        {
            HmiInfo = await SendAsync<RegisterAppInterfaceResponse>(raiRequest);
        }
        public async Task StartSession()
        {
            await _protocol.StartSessionAsync(ServiceType.Rpc);
        }

        private T ToRpcRequest<T>(ProtocolMessage message) 
        {
            if (message?.JsonSize > 0)
            {
                _logger.LogVerbose("Json", Encoding.ASCII.GetString(message.Payload));
                return JsonConvert.DeserializeObject<T>(Encoding.ASCII.GetString(message.Payload));
            }

            return default(T);
        }

        public void Dispose()
        {
            _transport?.Dispose();
            _protocol?.Dispose();
        }
    }
}