using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Converters;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Transport.Interfaces;

namespace SmartDeviceLink.Net.Transport
{
    public class TcpTransport : TransportBase
    {
        private ILogger _logger => Logger.SdlLogger;
        private readonly string _endpoint;
        private readonly int _port;
        private TcpClient _client = null;

        public TcpTransport(string endpoint,int port) 
        {
            _endpoint = endpoint;
            _port = port;
            _client = new TcpClient();
            
        }
        private Task ConnectAsync()
        {
            return _client.ConnectAsync(_endpoint,_port);
        }

        public override async Task SendAsync(TransportPacket packet)
        {
            if (IsDisposed) throw new ObjectDisposedException("Tcp Transport is disposed and cannot be used");
            if (!_client.Connected)
            {
                _logger.LogVerbose($"Connecting to {_endpoint}:{_port}");
                await ConnectAsync();
                _logger.LogVerbose($"Connected to {_endpoint}:{_port}");
            }
            
            var stream = _client.GetStream();
            var bytes = packet.ToTransportPacketFrame();
            await stream.WriteAsync(bytes, 0, bytes.Length);
            _logger.LogVerbose($"Sent {bytes.Length} to {_endpoint}:{_port}");
        }

        public override bool IsConnected => _client?.Connected ?? false;

        public override void Dispose()
        {
            if ( _client != null)
            {
                if(_client.Connected) _client.Dispose();
                _client = null;
            }
            base.Dispose();
        }

        public override void RecieveBytes()
        {
            
            while (!IsDisposed)
                if (_client != null && _client.Connected)
                {
                    var stream = _client.GetStream();
                    var buffer = new byte[1024];
                    var read =  stream.Read(buffer,0, 1024);
                    for(int i = 0; i < read; i++)
                        Parser.HandleByte(buffer[i]);
                }
        }
    }
    
}
