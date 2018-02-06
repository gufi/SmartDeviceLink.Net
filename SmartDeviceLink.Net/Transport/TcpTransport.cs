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
using SmartDeviceLink.Net.Protocol.Models;
using SmartDeviceLink.Net.Transport.Interfaces;

namespace SmartDeviceLink.Net.Transport
{
    public class TcpTransport : TransportBase
    {
        private ILogger _logger => Logger.SdlLogger;
        private readonly string _endpoint;
        private readonly int _port;
        private TcpClient _client = null;
        private bool _clientHasConnected = false;

        public TcpTransport(string endpoint,int port) 
        {
            _endpoint = endpoint;
            _port = port;
            _client = new TcpClient();
            _client.NoDelay = true;

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
                _clientHasConnected = true;
                _logger.LogVerbose($"Connected to {_endpoint}:{_port}");
            }
            
            //var stream = _client.GetStream();
            var bytes = packet.ToTransportPacketFrame();
            await _client.Client.SendAsync(new ArraySegment<byte>(bytes),SocketFlags.None);
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

        public override async Task ReceiveBytes(object o)
        {
            
            while (!IsDisposed)
            {
                if (_client != null && _client.Connected)
                {
                    var buffer = new byte[256];
                    _logger.LogVerbose("Reading");
                    var read = await _client.Client.ReceiveAsync(new ArraySegment<byte>(buffer),SocketFlags.None);
                    _logger.LogVerbose($"Read {read} bytes");
                    for (int i = 0; i < read; i++)
                        Parser.HandleByte(buffer[i]);
                }
                if(_clientHasConnected && _client != null && !_client.Connected)
                    _logger.LogWarning("client disconnected");
            }
            _logger.LogWarning("Disposed TcpTransport");
        }
    }
    
}
