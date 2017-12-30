using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Transport.Interfaces;

namespace SmartDeviceLink.Net.Transport
{
    public class TcpTransport : ITransport, IDisposable
    {
        private readonly string _endpoint;
        private readonly int _port;
        private readonly Action<TransportPacket> _onRecievedPacket;
        private readonly IPacketParser _parser;
        private readonly CancellationTokenSource _cTok;
        private readonly Task _recieveTask;
        private bool _isDisposed = false;
        public TcpTransport(string endpoint,int port, Action<TransportPacket> onRecievedPacket)
        {
            _endpoint = endpoint;
            _port = port;
            _onRecievedPacket = onRecievedPacket;
            _parser = new PacketParser(_onRecievedPacket);
            _client = new TcpClient();
            _cTok = new CancellationTokenSource();
            _recieveTask = Task.Run( () =>
            {
                if (_client != null && _client.Connected)
                {
                    var stream = _client.GetStream();
                    while (!_cTok.IsCancellationRequested)
                    {
                        var _byte = (byte)stream.ReadByte();
                        _parser.HandleByte(_byte);
                    }
                }
            },_cTok.Token);
        }
        private TcpClient _client = null;
        public Task ConnectAsync()
        {
            return _client.ConnectAsync(_endpoint,_port);

        }

        public Task SendBytesAsync(byte[] bytes)
        {
            if (_isDisposed) throw new ObjectDisposedException("Tcp Transport is disposed and cannot be used");
            if (_client.Connected)
            {
                var stream = _client.GetStream();
                return stream.WriteAsync(bytes, 0, bytes.Length);
            }
            return Task.FromResult(0);
        }
        public void Dispose()
        {
            if (!_isDisposed && _client != null)
            {
                if(_client.Connected) _client.Dispose();
                _client = null;
                _cTok.Cancel();
            }
            _isDisposed = true;
        }
    }
}
